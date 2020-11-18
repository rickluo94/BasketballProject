using Prism.Commands;
using First_MVVM.Notifications;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using IOModel;
using First_MVVM.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using EasyCardModel;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using DBModel;
using System.Data;
using System.Timers;
using RfidModel;

namespace First_MVVM.ViewModels
{
    public class CheckOutStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        private CheckOutModel _checkOutModel { get; set; }
        private DataTable _inventoryModel { get; set; }
        private EasyCard _easyCard = new EasyCard();
        private RFID _RFID = new RFID();
        private RFID_ReaderModel _rReaderModel { get; set; }
        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();
        private System.Timers.Timer OperationTimer;
        private System.Timers.Timer DoorCheckTimer;
        private System.Timers.Timer RFIDCheckTimer;
        private System.Timers.Timer ReaderTimer;

        #region Interface Property

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

        private int _doorCheckCounter;
        public int DoorCheckCounter
        {
            get { return _doorCheckCounter; }
            set { SetProperty(ref _doorCheckCounter, value); }
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

        private string _lockerSelectedIndex;

        public string LockerSelectedIndex
        {
            get { return _lockerSelectedIndex; }
            set { _lockerSelectedIndex = value; }
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

        

        public ObservableCollection<bool> ResStatus { get; private set; } = new ObservableCollection<bool>();
        private Business.BoxItemStatus _resStatusGroup = null;
        private List<bool> _resStatuslist { get;  set; }

        #endregion

        #region Interface Command 
        public DelegateCommand<WrapPanel> CheckOutStepTabLoadCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        public DelegateCommand PreviousTabCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }

        #endregion

        public CheckOutStepTabViewModel(Business.BoxItemStatus resStatusGroup)
        {
            _resStatusGroup = resStatusGroup;
            CheckOutStepTabLoadCmd = new DelegateCommand<WrapPanel>(CheckOutStepTabLoad);
            ExitCmd = new DelegateCommand(ExitInteraction);
            PreviousTabCmd = new DelegateCommand(PreviousTab);
            NextTabCmd = new DelegateCommand(NextTab);
            ReadCardCmd = new DelegateCommand(ReadCard);
        }

        private async void CheckOutStepTabLoad(WrapPanel LockerBox)
        {
            _inventoryModel = new DataTable();
            _checkOutModel = new CheckOutModel();
            _RFID.Connect();
            _rReaderModel = new RFID_ReaderModel();
            _rReaderModel.Status = true;
            _easyCard.SetDevicePort("COM7", 115200, 500); _easyCard.Open();
            NextStepIsEnabled = false;
            ReadCardIsEnabled = true;
            SelectedStepTabIndex = 0;
            if (await CheckAvailableUse() == true) { FillCabinetBtns(LockerBox);} else { MessageBox.Show("目前沒有可租借籃球"); ExitInteraction(); }
        }

        private async Task<bool> CheckAvailableUse()
        {
            //Get res status
            _resStatuslist = await _resStatusGroup.GetAll();
            foreach (bool res in _resStatuslist)
            {
                if (res == true)
                {
                    return true;
                }
            }
            return false;
        }

        private async void ReadCard()
        {
            ReadCardIsEnabled = false;
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            ReadCardIsEnabled = true;

            string _card_id = (string)JObject.Parse(Data)["result"]["card_id"];
            if (string.IsNullOrWhiteSpace(_card_id)) return;
            DataTable _rFID_UsersProfile = await _dBRead.RFID_Users(_card_id);

            if (_rFID_UsersProfile.Rows.Count > 0)
            {
                AccountStr = _rFID_UsersProfile.Rows[0]["RFID_user_id"].ToString();
                BalanceStr = (string)JObject.Parse(Data)["result"]["balance"];
                NoticeText = string.Empty;
                ReadCardIsEnabled = false;
                NextStepIsEnabled = true;

                DataTable _outstanding_Amount = await _dBRead.Outstanding_Amount(AccountStr);

                if (_outstanding_Amount.Rows.Count > 0)
                {
                    NoticeText = "尚有未付款";
                    NextStepIsEnabled = false;
                }

                int _notReturnedCheckOut = await _dBRead.NotReturnedCheckOut(AccountStr);

                if (_notReturnedCheckOut > 0)
                {
                    NoticeText = "尚有未歸還";
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

        private async void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "登入":
                    _checkOutModel.ID = _accountStr;
                    _checkOutModel.Balance = _balanceStr;

                    SetOperationTimer();
                    break;
                case "選擇櫃位":
                    _checkOutModel.LockerBosSelectedIndex = _lockerSelectedIndex;
                    //寫入櫃位EPC TID資料
                    _inventoryModel = await _dBRead.Inventory(_lockerSelectedIndex);
                    _checkOutModel.EPC = _inventoryModel.Rows[0]["Inventory_Items_EPC"].ToString();
                    _checkOutModel.TID = _inventoryModel.Rows[0]["Inventory_Items_TID"].ToString();

                    IO.Write(_checkOutModel.LockerBosSelectedIndex, IO.UnLock);
                    NoticeText = "提醒您球櫃開起後未關閉視同已借出";

                    //在此函式判斷是否開始計費 or 操作逾時
                    SetReaderTimer();
                    SetDoorCheckTimer();
                    break;
                default:
                    break;
            }
        }

        private void FillCabinetBtns(WrapPanel LockerBox)
        {
            LockerBox.Children.Clear();
            List<Button> CabinetButton = new List<Button>
            {
                new Button(),
                new Button(),
                new Button(),
                new Button(),
                new Button(),
                new Button(),
                new Button()
            };
            for (int i = 0; i < CabinetButton.Count; i++)
            {
                CabinetButton[i].Content = $"A{Math.Abs(i + 1)}";
                CabinetButton[i].Width = 170;
                CabinetButton[i].Height = 170;
                CabinetButton[i].IsEnabled = _resStatuslist[i];
                CabinetButton[i].Margin = new Thickness() { Bottom = 10, Left = 10, Right = 10, Top = 10 };
                CabinetButton[i].Click += new System.Windows.RoutedEventHandler(CabinetBtns_Click);
                LockerBox.Children.Add(CabinetButton[i]);
            }
        }

        public void CabinetBtns_Click(object sender, EventArgs e)
        {
            var parameter = sender as Button;
            LockerSelectedIndex = parameter.Content.ToString();
            NextStepIsEnabled = true;
        }

        private void ExitInteraction()
        {
            AccountStr = null;
            BalanceStr = null;
            NoticeText = null;
            DoorCheckCounter = 0;
            _easyCard.Close();
            _RFID.Disconnect();
            FinishInteraction?.Invoke();
        }

        private async Task CallBazz()
        {
            IO.Write("Buzz", IO.UnLock);
            await Task.Delay(1000);
            IO.Write("Buzz", IO.Lock);
            await Task.Delay(1000);
        }

        #region Set Subscribe Event

        private void SetOperationTimer()
        {
            Counter = 0;
            OperationTimer = new System.Timers.Timer(1000);
            OperationTimer.Elapsed += OnTimedOperationEvent;
            OperationTimer.AutoReset = true;
            OperationTimer.Enabled = true;
        }

        private void SetDoorCheckTimer()
        {
            Counter = 0;
            DoorCheckTimer = new System.Timers.Timer(1000);
            DoorCheckTimer.Elapsed += OnTimedDoorCheckEvent;
            DoorCheckTimer.AutoReset = true;
            DoorCheckTimer.Enabled = true;
        }

        private void SetRFIDCheckTimer()
        {
            Counter = 0;
            RFIDCheckTimer = new System.Timers.Timer(1000);
            RFIDCheckTimer.Elapsed += OnTimedRFIDCheckEvent;
            RFIDCheckTimer.AutoReset = true;
            RFIDCheckTimer.Enabled = true;
        }

        private void SetReaderTimer()
        {
            ReaderTimer = new System.Timers.Timer(1000);
            ReaderTimer.Elapsed += OnTimedReaderEvent;
            ReaderTimer.AutoReset = true;
            ReaderTimer.Enabled = true;
        }

        #endregion


        #region Unsubscribe Event

        private void UnsubscribeOperationEvent()
        {
            OperationTimer.Elapsed -= OnTimedOperationEvent;
            OperationTimer.Close();
        }

        private void UnsubscribeDoorCheckEvent()
        {
            DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
            DoorCheckTimer.Close();
        }

        private void UnsubscribeRFIDCheckEvent()
        {
            RFIDCheckTimer.Elapsed -= OnTimedRFIDCheckEvent;
            RFIDCheckTimer.Close();
        }

        private void UnsubscribeReaderEvent()
        {
            ReaderTimer.Elapsed -= OnTimedReaderEvent;
            ReaderTimer.Close();
        }

        #endregion


        #region On Subscribe Event

        private void OnTimedOperationEvent(Object source, ElapsedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_checkOutModel.LockerBosSelectedIndex))
            {
                Counter += 1;
                if (Counter == 20)
                {
                    Counter = 0;

                    UnsubscribeOperationEvent();

                    SelectedStepTabName = "操作逾時";
                }
            }
            else
            {
                Counter = 0;

                UnsubscribeOperationEvent();
            }
            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";
        }

        private void OnTimedDoorCheckEvent(Object source, ElapsedEventArgs e)
        {
            if (IO.Read(_checkOutModel.LockerBosSelectedIndex) == IO.DoorLock)
            {
                Counter += 1;
                if (Counter == 15)
                {
                    Counter = 0;

                    UnsubscribeDoorCheckEvent();

                    SelectedStepTabName = "操作逾時";
                    IO.Write(_checkOutModel.LockerBosSelectedIndex, IO.Lock);

                    UnsubscribeReaderEvent();
                }
            }
            else
            {
                Counter = 0;

                UnsubscribeDoorCheckEvent();

                IO.Write(_checkOutModel.LockerBosSelectedIndex, IO.Lock);

                ///資料庫新增會員借出紀錄依據_rentalModel內資料寫入//

                SetRFIDCheckTimer();
            }
            NoticeText = $"剩餘操作時間 {(15 - Counter)} sec";
        }

        private void OnTimedRFIDCheckEvent(Object source, ElapsedEventArgs e)
        {
            if (IO.Read(_checkOutModel.LockerBosSelectedIndex) == IO.DoorOpen)
            {
                Counter += 1;
                if (Counter == 50)
                {
                    Counter = 0;

                    UnsubscribeRFIDCheckEvent();

                    //寫入借出紀錄
                    _dBWrite.Inventory(_checkOutModel.LockerBosSelectedIndex, 0);
                    _dBWrite.Take_History(_checkOutModel.ID, _checkOutModel.LockerBosSelectedIndex, _checkOutModel.EPC, _checkOutModel.TID);

                    UnsubscribeReaderEvent();

                    SelectedStepTabName = "租借完成";
                }
            }
            else
            {
                //此處需要引用RFID模組
                if (_rReaderModel.Status == false || DoorCheckCounter >= 3)
                {
                    Counter = 0;

                    UnsubscribeRFIDCheckEvent();

                    //寫入借出紀錄
                    _dBWrite.Inventory(_checkOutModel.LockerBosSelectedIndex, 0);
                    _dBWrite.Take_History(_checkOutModel.ID, _checkOutModel.LockerBosSelectedIndex, _checkOutModel.EPC, _checkOutModel.TID);

                    UnsubscribeReaderEvent();


                    SelectedStepTabName = "租借完成";
                }
                else
                {
                    Counter = 0;

                    UnsubscribeRFIDCheckEvent();

                    IO.Write(_checkOutModel.LockerBosSelectedIndex, IO.UnLock);
                    SetDoorCheckTimer(); DoorCheckCounter += 1;
                    SelectedStepTabName = "請取球";
                }
            }
            NoticeText = $"剩餘操作時間 {(50 - Counter)} sec";
        }

        private void OnTimedReaderEvent(Object source, ElapsedEventArgs e)
        {
            Tuple<bool, string, string> result = _RFID.ScannAndRead(_checkOutModel.LockerBosSelectedIndex, _checkOutModel.EPC);
            _rReaderModel.Status = result.Item1;
            _rReaderModel.EPC = result.Item2;
            _rReaderModel.TID = result.Item3;
            ReaderStatusStr = _rReaderModel.Status.ToString();
            if (_rReaderModel.Status == true)
            {
                ReaderStatusStr = "等待取球";
            }
            else
            {
                CallBazz();
                ReaderStatusStr = "請關門";
            }
        }

        #endregion


        #region MainWindow Interactive
        public Action FinishInteraction { get; set; }

        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }

        #endregion

    }
}
