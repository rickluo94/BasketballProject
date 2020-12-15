using Prism.Commands;
using First_MVVM.Notifications;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using EasyCardModel;
using System.Data;
using DBModel;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using First_MVVM.Models;
using IOModel;
using System.Timers;
using Prism.Services.Dialogs;
using Prism.Regions;

namespace First_MVVM.ViewModels
{
    public class MemberServicePageViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private readonly IRegionManager _regionManager;
        private MemberServiceModel _memberServiceModel { get; set; }
        private EasyCard _easyCard = new EasyCard();

        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();

        private System.Timers.Timer PumpStartTimer;
        private System.Timers.Timer DoorCheckTimer;

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }

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

        private bool _pumpBoxStartIsEnable;
        public bool PumpBoxStartIsEnable
        {
            get { return _pumpBoxStartIsEnable; }
            set { SetProperty(ref _pumpBoxStartIsEnable, value); }
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

        private string _accountStr;
        public string AccountStr
        {
            get { return _accountStr; }
            set { SetProperty(ref _accountStr, value); }
        }

        private string _card_id;
        public string Card_id
        {
            get { return _card_id; }
            set { SetProperty(ref _card_id, value); }
        }

        private string _balanceStr;
        public string BalanceStr
        {
            get { return _balanceStr; }
            set { SetProperty(ref _balanceStr, value); }
        }

        private string _noticeText;
        public string NoticeText
        {
            get { return _noticeText; }
            set { SetProperty(ref _noticeText, value); }
        }

        private bool _nextStepIsEnabled;
        public bool NextStepIsEnabled
        {
            get { return _nextStepIsEnabled; }
            set { SetProperty(ref _nextStepIsEnabled, value); }
        }

        private string _amountStr;
        public string AmountStr
        {
            get { return _amountStr; }
            set { SetProperty(ref _amountStr, value); }
        }

        private string _sNStr;
        public string SNStr
        {
            get { return _sNStr; }
            set { SetProperty(ref _sNStr, value); }
        }

        private string _pumpBoxStatus;
        public string PumpBoxStatus
        {
            get { return _pumpBoxStatus; }
            set { SetProperty(ref _pumpBoxStatus, value); }
        }

        #endregion

        #region Interface Command
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand MemberServicePageLoadCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }
        public DelegateCommand DebitCmd { get; private set; }
        public DelegateCommand GoToDebitPageCmd { get; private set; }
        public DelegateCommand GoToPumpPageCmd { get; private set; }
        public DelegateCommand CancelAccountCmd { get; private set; }
        public DelegateCommand PumpBoxStartCmd { get; private set; }

        #endregion

        public MemberServicePageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            MemberServicePageLoadCmd = new DelegateCommand(MemberServicePageLoad);
            ExitCmd = new DelegateCommand(ExitInteraction);
            NextTabCmd = new DelegateCommand(NextTab);
            ReadCardCmd = new DelegateCommand(ReadCard);
            GoToDebitPageCmd = new DelegateCommand(GoToDebitPage);
            DebitCmd = new DelegateCommand(Charge);
            GoToPumpPageCmd = new DelegateCommand(GoToPumpPage);
            PumpBoxStartCmd = new DelegateCommand(PumpBoxStart);
            CancelAccountCmd = new DelegateCommand(CancelAccount);
        }

        private void MemberServicePageLoad()
        {
            PumpBoxStartIsEnable = true;
            DebitIsEnabled = true;
            ReadCardIsEnabled = true;
            NextStepIsEnabled = false;
            SelectedStepTabIndex = 0;
            _memberServiceModel = new MemberServiceModel();
            _easyCard.SetDevicePort("COM7", 115200, 500); _easyCard.Open();
        }

        private void ExitInteraction()
        {
            AccountStr = null;
            BalanceStr = null;
            NoticeText = null;
            Card_id = null;
            AmountStr = null;
            SNStr = null;
            PumpBoxStatus = null;
            _easyCard.Close();
            _regionManager.Regions["ContentRegion"].RemoveAll();
        }

        private void GoToDebitPage()
        {
            if (!string.IsNullOrWhiteSpace(_memberServiceModel.Amount.ToString()))
            {
                NoticeText = $"未結清款項 ${_memberServiceModel.Amount}";
                SelectedStepTabName = "付款";
            }
            else
            {
                NoticeText = "未結清款項 $0";
            }
        }

        private void GoToPumpPage()
        {
            SelectedStepTabName = "打氣";
        }

        private async void CancelAccount()
        {
            NoticeText = string.Empty;
            int _notReturnedCheckOut = await _dBRead.NotReturnedCheckOut(_memberServiceModel.SN);
            DataTable _outstanding_Amount = await _dBRead.Charge_History(_memberServiceModel.SN);

            if (_notReturnedCheckOut > 0)
            {
                NoticeText = "尚有未歸還";
            }
            if (_outstanding_Amount.Rows.Count > 0)
            {
                NoticeText = "尚有未付款，不可註銷帳號";
            }

            if (_notReturnedCheckOut == 0 && _outstanding_Amount.Rows.Count == 0)
            {
                bool CancelAccount = await _dBWrite.Customer_info_UPDATE(_memberServiceModel.SN, "Status", "2");
                if (CancelAccount == true)
                {
                    NoticeText = "感謝您的使用";
                    SelectedStepTabName = "完成";
                }
            }
        }

        private void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "登入":
                    _memberServiceModel.ID = _accountStr;
                    _memberServiceModel.CardID = _card_id;
                    _memberServiceModel.Balance = _balanceStr;
                    //測試金額
                    _memberServiceModel.Amount = 0;
                    break;
                case "服務選單":


                    break;
                case "付款":

                    break;
                case "打氣":

                    break;
                case "操作逾時":

                    break;
                case "完成":

                    break;
                default:
                    break;
            }
        }

        private void NextTab()
        {
            FillProfile();
            NoticeText = null;
            NextStepIsEnabled = false;
            SelectedStepTabIndex += 1;
        }

        private async void ReadCard()
        {
            ReadCardIsEnabled = false;
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            ReadCardIsEnabled = true;

            Card_id = (string)JObject.Parse(Data)["result"]["card_id"];
            if (string.IsNullOrWhiteSpace(_card_id)) return;
            DataTable RFIDS = await _dBRead.RFIDS(_card_id);

            if (RFIDS.Rows.Count > 0)
            {
                DataTable Customer_info = await _dBRead.Customer_info(RFIDS.Rows[0]["SN"].ToString());
                _memberServiceModel.SN = Customer_info.Rows[0]["SN"].ToString();

                if (Customer_info.Rows[0]["Status"].ToString() == "1")
                {

                    AccountStr = Customer_info.Rows[0]["user_id"].ToString();
                    BalanceStr = (string)JObject.Parse(Data)["result"]["balance"];

                    DataTable _outstanding_Amount = await _dBRead.Charge_History(_memberServiceModel.SN);
                    DataTable _checkOut_History = await _dBRead.Take_History(_memberServiceModel.SN);
                    if (_outstanding_Amount.Rows.Count > 0)
                    {
                        NoticeText = "尚有未付款";
                        AmountStr = _outstanding_Amount.Rows[0]["Charge_amount"].ToString();
                        SNStr = _outstanding_Amount.Rows[0]["Charge_SN"].ToString();
                    }

                    ReadCardIsEnabled = false;
                    NextStepIsEnabled = true;
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

        private async void Charge()
        {
            //測試用
            _memberServiceModel.Amount = 0;

            if (!string.IsNullOrWhiteSpace(_memberServiceModel.Amount.ToString()))
            {
                DebitIsEnabled = false;
                string _chargeResult = await Task.Run<string>(() => { return _easyCard.Charge_request(_memberServiceModel.Amount); });

                string _isSuccess = (string)JObject.Parse(_chargeResult)["is_success"];

                if (_isSuccess == "True")
                {
                    _dBWrite.Charge_History_UPDATE(_sNStr);
                    _memberServiceModel.DebitStatus = "成功";
                    NoticeText = "扣款成功";
                    SelectedStepTabName = "完成";
                }
                else
                {
                    NoticeText = "扣款失敗";
                }

                DebitIsEnabled = true;
            }
        }

        private async Task CallBazz()
        {
            IO.Write("Buzz", IO.UnLock);
            await Task.Delay(1000);
            IO.Write("Buzz", IO.Lock);
            await Task.Delay(1000);
        }

        private async void PumpBoxStart()
        {
            _memberServiceModel.CheckOut = DateTime.Now;
            bool insertPumpHistory = await _dBWrite.Pump_History(_memberServiceModel.SN, 0, "A8");
            if (insertPumpHistory == true)
            {
                IO.Write("A8", IO.UnLock);

                CallBazz();

                SetDoorCheckTimer();

                PumpBoxStartIsEnable = false;
            }
        }

        private async Task UpDatePumpHistory(double Time_usage)
        {
            bool UpDatePumpHistory = await _dBWrite.Pump_History_UPDATE(_memberServiceModel.SN, "Time_usage", Time_usage);
        }

        #region Unsubscribe Event

        private void UnsubscribeDoorCheckEvent()
        {
            DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
            DoorCheckTimer.Close();
        }

        private void UnsubscribePumpStartEvent()
        {
            PumpStartTimer.Elapsed -= OnTimePumpStartEvent;
            PumpStartTimer.Close();
        }

        #endregion


        #region Set Subscribe Event

        private void SetDoorCheckTimer()
        {
            Counter = 0;
            DoorCheckTimer = new System.Timers.Timer(1000);
            DoorCheckTimer.Elapsed += OnTimedDoorCheckEvent;
            DoorCheckTimer.AutoReset = true;
            DoorCheckTimer.Enabled = true;
        }

        private void SetPumpStartTimer()
        {
            Counter = 0;
            PumpStartTimer = new System.Timers.Timer(1000);
            PumpStartTimer.Elapsed += OnTimePumpStartEvent;
            PumpStartTimer.AutoReset = true;
            PumpStartTimer.Enabled = true;
        }

        #endregion


        #region On Subscribe Event

        private void OnTimedDoorCheckEvent(Object source, ElapsedEventArgs e)
        {
            if (IO.Read("A8") == IO.DoorLock)
            {
                Counter += 1;
                if (Counter == 15)
                {
                    Counter = 0;

                    UnsubscribeDoorCheckEvent();

                    SelectedStepTabName = "完成";

                    IO.Write("A8", IO.Lock);

                    NoticeText = string.Empty;
                }

                NoticeText = $"請開啟A8打氣櫃 {(15 - Counter)} sec";
            }
            else
            {
                Counter = 0;

                NoticeText = string.Empty;

                UnsubscribeDoorCheckEvent();

                IO.Write("A8", IO.Lock);

                SetPumpStartTimer();
            }

        }

        private void OnTimePumpStartEvent(Object source, ElapsedEventArgs e)
        {
            _counter += 1;

            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";

            if (IO.Read("Pump") == IO.PumpON)
            {
                IO.Write("Pump", IO.UnLock);
            }
            else
            {
                IO.Write("Pump", IO.Lock);
            }

            if (Counter == 20 || IO.Read("A8") == IO.DoorLock)
            {
                _memberServiceModel.CheckIn = DateTime.Now;
                TimeSpan Ts = _memberServiceModel.CheckIn - _memberServiceModel.CheckOut;
                double UseTotalSec = Ts.TotalSeconds;
                UpDatePumpHistory(UseTotalSec);

                UnsubscribePumpStartEvent();

                IO.Write("A8", IO.Lock);

                IO.Write("Pump", IO.Lock);

                NoticeText = string.Empty;

                SelectedStepTabName = "完成";
            }

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
