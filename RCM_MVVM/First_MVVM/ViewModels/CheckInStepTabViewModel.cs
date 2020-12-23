using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Interactivity.InteractionRequest;

using First_MVVM.Models;
using First_MVVM.Notifications;

using System;
using IOModel;
using DBModel;
using EasyCardModel;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Timers;
using RfidModel;


namespace First_MVVM.ViewModels
{
    public class CheckInStepTabViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private readonly IRegionManager _regionManager;
        private CheckInModel _checkInModel { get; set; }
        private EasyCard _easyCard = new EasyCard();

        private RFID _RFID = new RFID();
        private RFID_ReaderModel _rReaderModel { get; set; }

        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();
        private LC_DBWrite _lC_DBWrite = new LC_DBWrite();
        private System.Timers.Timer ReaderTimer;
        private System.Timers.Timer DoorCheckTimer;
        private System.Timers.Timer DoorCheckWithRFIDVerifyTimer;
        private System.Timers.Timer DebitCheckTimer;

        #region Interface Property
        private bool _debitIsEnabled;
        public bool DebitIsEnabled
        {
            get { return _debitIsEnabled; }
            set { SetProperty(ref _debitIsEnabled, value); }
        }

        private bool _readCardIsEnabled;
        public bool ReadCardIsEnabled
        {
            get { return _readCardIsEnabled; }
            set { SetProperty(ref _readCardIsEnabled, value); }
        }

        private string _readerStatusStr;
        public string ReaderStatusStr
        {
            get { return _readerStatusStr; }
            set { SetProperty(ref _readerStatusStr, value); }
        }

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }

        private string _noticeText;

        public string NoticeText
        {
            get { return _noticeText; }
            set { SetProperty(ref _noticeText, value); }
        }

        private int _selectedStepTabIndex;

        public int SelectedStepTabIndex
        {
            get { return _selectedStepTabIndex; }
            set { SetProperty(ref _selectedStepTabIndex, value); }
        }

        private string _selectedStepTabName;

        public string SelectedStepTabName
        {
            get { return _selectedStepTabName; }
            set { SetProperty(ref _selectedStepTabName, value); }
        }

        private bool _nextStepIsEnabled;

        public bool NextStepIsEnabled
        {
            get { return _nextStepIsEnabled; }
            set { SetProperty(ref _nextStepIsEnabled, value); }
        }

        private string _accountStr;

        public string AccountStr
        {
            get { return _accountStr; }
            set { SetProperty(ref _accountStr, value); }
        }

        private string _balanceStr;

        public string BalanceStr
        {
            get { return _balanceStr; }
            set { SetProperty(ref _balanceStr, value); }
        }

        private string _lockerBoxSelectedIndex;

        public string LockerBoxSelectedIndex
        {
            get { return _lockerBoxSelectedIndex; }
            set { SetProperty(ref _lockerBoxSelectedIndex, value); }
        }

        private string _outTimeStr;
        public string OutTimeStr
        {
            get { return _outTimeStr; }
            set { SetProperty(ref _outTimeStr, value); }
        }

        private string _inTimeStr;
        public string InTimeStr
        {
            get { return _inTimeStr; }
            set { SetProperty(ref _inTimeStr, value); }
        }

        private string _amountStr;
        public string AmountStr
        {
            get { return _amountStr; }
            set { SetProperty(ref _amountStr, value); }
        }
        #endregion

        #region Interface Command 
        public DelegateCommand CheckInStepTabLoadCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        public DelegateCommand PreviousTabCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }
        public DelegateCommand DebitCmd { get; private set; }

        #endregion

        public CheckInStepTabViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            CheckInStepTabLoadCmd = new DelegateCommand(CheckInStepTabLoad);
            ExitCmd = new DelegateCommand(ExitInteraction);
            NextTabCmd = new DelegateCommand(NextTab); PreviousTabCmd = new DelegateCommand(PreviousTab);
            ReadCardCmd = new DelegateCommand(ReadCard);
            DebitCmd = new DelegateCommand(Charge);
        }

        private void CheckInStepTabLoad()
        {
            _checkInModel = new CheckInModel();

            _RFID.Connect();
            _rReaderModel = new RFID_ReaderModel();
            _rReaderModel.Status = false;

            _easyCard.SetDevicePort("COM7", 115200, 500); _easyCard.Open();
            DebitIsEnabled = true;
            ReadCardIsEnabled = true;
            NextStepIsEnabled = false;
            SelectedStepTabIndex = 0;
        }

        private async void ReadCard()
        {
            ReadCardIsEnabled = false;
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            ReadCardIsEnabled = true;

            string _card_id = (string)JObject.Parse(Data)["result"]["card_id"];
            if (string.IsNullOrWhiteSpace(_card_id)) return;
            DataTable RFIDS = await _dBRead.RFIDS(_card_id);

            if (RFIDS.Rows.Count > 0)
            {
                DataTable Customer_info = await _dBRead.Customer_info(RFIDS.Rows[0]["SN"].ToString());
                _checkInModel.SN = Customer_info.Rows[0]["SN"].ToString();

                if (Customer_info.Rows[0]["Status"].ToString() == "1")
                {
                    AccountStr = Customer_info.Rows[0]["user_id"].ToString();
                    BalanceStr = (string)JObject.Parse(Data)["result"]["balance"];

                    DataTable _outstanding_Amount = await _dBRead.Charge_History(_checkInModel.SN);
                    DataTable _checkOut_History = await _dBRead.Take_History(_checkInModel.SN);
                    if (_outstanding_Amount.Rows.Count > 0)
                    {
                        NoticeText = "尚有未付款";
                        NextStepIsEnabled = false;
                    }
                    else
                    {
                        if (_checkOut_History.Rows.Count > 0)
                        {
                            OutTimeStr = _checkOut_History.Rows[0]["Take_CheckOut"].ToString();
                            LockerBoxSelectedIndex = _checkOut_History.Rows[0]["Take_BoxName"].ToString();
                            _checkInModel.Take_SN = _checkOut_History.Rows[0]["Take_SN"].ToString();
                            _checkInModel.CardID = _card_id;
                            NoticeText = string.Empty;
                            ReadCardIsEnabled = false;
                            NextStepIsEnabled = true;
                        }
                    }

                }
                else
                {
                    NoticeText = "帳號已停用";
                    NextStepIsEnabled = false;
                }


            }
            else
            {
                AccountStr = string.Empty;
                BalanceStr = string.Empty;
                NoticeText = "查無此會員";
                NextStepIsEnabled = false;
            }
        }

        private void NextTab()
        {
            FillProfile();
            NoticeText = null;
            NextStepIsEnabled = false;
            SelectedStepTabIndex += 1;
        }

        private void PreviousTab()
        {
            SelectedStepTabIndex -= 1;
        }

        private void ExitInteraction()
        {
            AccountStr = null;
            BalanceStr = null;
            NoticeText = null;
            _easyCard.Close();
            _RFID.Disconnect();
            _regionManager.Regions["ContentRegion"].RemoveAll();
        }

        private void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "登入":
                    _checkInModel.ID = _accountStr;
                    _checkInModel.Balance = _balanceStr;
                    _checkInModel.OutTime = Convert.ToDateTime(_outTimeStr);
                    _checkInModel.LockerBoxSelectedIndex = _lockerBoxSelectedIndex;

                    break;
                case "借出紀錄":
                    DateTime inDate = DateTime.Now;
                    InTimeStr = inDate.ToString();
                    _checkInModel.InTime = inDate;
                    TimeSpan Duration = _checkInModel.InTime.Subtract(_checkInModel.OutTime).Duration();
                    _checkInModel.HoursUse = Duration.TotalMinutes.ToString();

                    IO.Write(_checkInModel.LockerBoxSelectedIndex, IO.UnLock);

                    SetDoorCheckTimer();
                    break;
                case "歸還":
                    //計算付款金額填入amount
                    _checkInModel.Amount = 0;

                    SetDebitCheckTimer();
                    break;
                case "付款":


                    
                    break;
                default:
                    break;
            }
        }

        private async void Charge()
        {
            DataTable _charge_History = await _dBRead.Charge_History(_checkInModel.SN);
            int _amount = _checkInModel.Amount;

            if (_charge_History.Rows.Count > 0)
            {
                string _charge_SN = _charge_History.Rows[0]["Charge_SN"].ToString();

                DebitIsEnabled = false;
                string _chargeResult = await Task.Run<string>(() => { return _easyCard.Charge_request(_amount); });

                string _isSuccess = (string)JObject.Parse(_chargeResult)["is_success"];

                if (_isSuccess == "True")
                {
                    _dBWrite.Charge_History_UPDATE(_charge_SN);
                    _checkInModel.DebitStatus = "成功";
                }
                else
                {
                    DebitIsEnabled = true;
                }
            }
        }

        private async Task CallBazz()
        {
            IO.Write("Buzz", IO.UnLock);
            await Task.Delay(1000);
            IO.Write("Buzz", IO.Lock);
            await Task.Delay(1000);
        }

        #region Set Subscribe Event

        private void SetDoorCheckTimer()
        {
            Counter = 0;
            DoorCheckTimer = new System.Timers.Timer(1000);
            DoorCheckTimer.Elapsed += OnTimedDoorCheckEvent;
            DoorCheckTimer.AutoReset = true;
            DoorCheckTimer.Enabled = true;
        }

        private void SetDoorCheckWithRFIDVerifyTimer()
        {
            Counter = 0;
            DoorCheckWithRFIDVerifyTimer = new System.Timers.Timer(1000);
            DoorCheckWithRFIDVerifyTimer.Elapsed += OnTimedDoorCheckWithRFIDVerifyEvent;
            DoorCheckWithRFIDVerifyTimer.AutoReset = true;
            DoorCheckWithRFIDVerifyTimer.Enabled = true;
        }

        private void SetReaderTimer()
        {
            ReaderTimer = new System.Timers.Timer(1000);
            ReaderTimer.Elapsed += OnTimedReaderEvent;
            ReaderTimer.AutoReset = true;
            ReaderTimer.Enabled = true;
        }

        private void SetDebitCheckTimer()
        {
            Counter = 0;
            DebitCheckTimer = new System.Timers.Timer(1000);
            DebitCheckTimer.Elapsed += OnTimedDebitCheckTimerEvent;
            DebitCheckTimer.AutoReset = true;
            DebitCheckTimer.Enabled = true;
        }

        #endregion


        #region Unsubscribe Event

        private void UnsubscribeDoorCheckEvent()
        {
            DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
            DoorCheckTimer.Close();
        }

        private void UnsubscribeDoorCheckWithRFIDVerify()
        {
            DoorCheckWithRFIDVerifyTimer.Elapsed -= OnTimedDoorCheckWithRFIDVerifyEvent;
            DoorCheckWithRFIDVerifyTimer.Close();
        }

        private void UnsubscribeReaderEvent()
        {
            ReaderTimer.Elapsed -= OnTimedReaderEvent;
            ReaderTimer.Close();
        }

        public void UnsubscribeDebitCheckTimerEvent()
        {
            DebitCheckTimer.Elapsed -= OnTimedDebitCheckTimerEvent;
            DebitCheckTimer.Close();
        }

        #endregion


        #region On Subscribe Event

        private void OnTimedDoorCheckEvent(Object source, ElapsedEventArgs e)
        {
            if (IO.Read(_checkInModel.LockerBoxSelectedIndex) == IO.DoorLock)
            {
                Counter += 1;
                if (Counter == 20)
                {
                    Counter = 0;

                    UnsubscribeDoorCheckEvent();

                    SelectedStepTabName = "操作逾時";

                    IO.Write(_checkInModel.LockerBoxSelectedIndex, IO.Lock);
                }
            }
            else
            {
                Counter = 0;

                UnsubscribeDoorCheckEvent();

                IO.Write(_checkInModel.LockerBoxSelectedIndex, IO.Lock);

                //正常流程
                SetReaderTimer();
                SetDoorCheckWithRFIDVerifyTimer();
            }

            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";
        }

        private void OnTimedDoorCheckWithRFIDVerifyEvent(Object source, ElapsedEventArgs e)
        {
            if (IO.Read(_checkInModel.LockerBoxSelectedIndex) == IO.DoorOpen)
            {
                Counter += 1;
                if (Counter == 30)
                {
                    Counter = 0;

                    UnsubscribeDoorCheckWithRFIDVerify();

                    UnsubscribeReaderEvent();

                    SelectedStepTabName = "操作逾時";
                }
            }
            else
            {
                //此處需要引用RFID模組
                if (_rReaderModel.Status == true && IO.Read(_checkInModel.LockerBoxSelectedIndex) == IO.DoorLock)
                {
                    Counter = 0;

                    UnsubscribeDoorCheckWithRFIDVerify();

                    UnsubscribeReaderEvent();

                    //建立Charge_History
                    _lC_DBWrite.Inventory(_checkInModel.LockerBoxSelectedIndex, 1);
                    _dBWrite.Take_History_UPDATE(_checkInModel.Take_SN, _checkInModel.InTime.ToString());
                    ////存入歷史紀錄
                    _dBWrite.Charge_History(_checkInModel.SN, _checkInModel.Amount, _checkInModel.HoursUse, _checkInModel.CardID);

                    NoticeText = "歸還成功，請點擊付款";
                    NextStepIsEnabled = true;
                }
                else
                {
                    Counter = 0;

                    UnsubscribeDoorCheckWithRFIDVerify();

                    UnsubscribeReaderEvent();

                    SelectedStepTabName = "歸還失敗";
                }
            }

            NoticeText = $"剩餘操作時間 {(30 - Counter)} sec";
        }

        private void OnTimedReaderEvent(Object source, ElapsedEventArgs e)
        {
            Tuple<bool, string, string> result = _RFID.ScannAndRead(_checkInModel.LockerBoxSelectedIndex, _checkInModel.EPC);
            _rReaderModel.Status = result.Item1;
            _rReaderModel.EPC = result.Item2;
            _rReaderModel.TID = result.Item3;
            if (_rReaderModel.Status == true)
            {
                CallBazz();
                ReaderStatusStr = "請關門";
            }
            else
            {
                ReaderStatusStr = "請將球置入";
            }
        }

        private void OnTimedDebitCheckTimerEvent(Object source, ElapsedEventArgs e)
        {
            Counter += 1;
            if (Counter == 50)
            {
                Counter = 0;

                UnsubscribeDebitCheckTimerEvent();

                SelectedStepTabName = "付款失敗";
            }
            else
            {
                if (_checkInModel.DebitStatus == "成功")
                {
                    Counter = 0;
                    UnsubscribeDebitCheckTimerEvent();

                    SelectedStepTabName = "歸還完成";
                }
            }

            NoticeText = $"剩餘操作時間 {(50 - Counter)} sec";
        }

        #endregion


        #region MainWindow Interactive

        public bool KeepAlive
        {
            get
            {
                return false;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #endregion
    }
}
