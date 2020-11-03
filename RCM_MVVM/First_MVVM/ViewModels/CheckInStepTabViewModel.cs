using Prism.Commands;
using Prism.Mvvm;
using First_MVVM.Models;
using First_MVVM.Notifications;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using IOModel;
using DBModel;
using EasyCardModel;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Timers;
using MySqlX.XDevAPI.Common;

namespace First_MVVM.ViewModels
{
    public class CheckInStepTabViewModel : BindableBase , IInteractionRequestAware
    {
        private CheckInModel _checkInModel { get; set; }
        private IO _IO = new IO();
        private EasyCard _easyCard = new EasyCard();
        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();
        //private System.Timers.Timer OperationTimer;
        private System.Timers.Timer DoorCheckTimer;
        private System.Timers.Timer DoorCheckWithRFIDVerifyTimer;
        private System.Timers.Timer DebitCheckTimer;

        #region Interface Property

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

        private string _lockerSelectedIndex;

        public string LockerSelectedIndex
        {
            get { return _lockerSelectedIndex; }
            set { SetProperty(ref _lockerSelectedIndex, value); }
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

        public CheckInStepTabViewModel()
        {
            CheckInStepTabLoadCmd = new DelegateCommand(CheckInStepTabLoad);
            ExitCmd = new DelegateCommand(ExitInteraction);
            NextTabCmd = new DelegateCommand(NextTab); PreviousTabCmd = new DelegateCommand(PreviousTab);
            ReadCardCmd = new DelegateCommand(ReadCard);
            DebitCmd = new DelegateCommand(Charge);
        }

        private void CheckInStepTabLoad()
        {
            NextStepIsEnabled = true;
            SelectedStepTabIndex = 0;
        }

        private async void ReadCard()
        {
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            string _card_id = (string)JObject.Parse(Data)["result"]["card_id"];
            if (string.IsNullOrWhiteSpace(_card_id)) return;
            DataTable _member_Profile = await _dBRead.Verify_TheMember(_card_id);

            if (_member_Profile.Rows.Count > 0)
            {
                AccountStr = _member_Profile.Rows[0]["UserID"].ToString();
                BalanceStr = (string)JObject.Parse(Data)["result"]["balance"];

                DataTable _outstanding_Amount = await _dBRead.Outstanding_Amount(AccountStr);
                DataTable _checkOut_History = await _dBRead.CheckOut_History(AccountStr);
                if (_outstanding_Amount.Rows.Count > 0)
                {
                    NoticeText = "尚有未付款";
                    NextStepIsEnabled = false;
                }
                else
                {
                    if (_checkOut_History.Rows.Count >0)
                    {
                        OutTimeStr = _checkOut_History.Rows[0]["OutTIme"].ToString();
                        LockerSelectedIndex = _checkOut_History.Rows[0]["LockerSelectedIndex"].ToString();
                        NoticeText = string.Empty;
                        NextStepIsEnabled = true;
                    }
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
            FinishInteraction?.Invoke();
        }

        private void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "登入":
                    _checkInModel.ID = _accountStr;
                    _checkInModel.Balance = _balanceStr;
                    _checkInModel.OutTime = _outTimeStr;
                    _checkInModel.LockerSelectedIndex = _lockerSelectedIndex;

                    break;
                case "借出紀錄":
                    _checkInModel.InTime = "現在時間";

                    _IO.Write(_checkInModel.LockerSelectedIndex, _IO.UnLock);

                    SelectedStepTabName = "歸還";

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

        private void SetDoorCheckTimer()
        {
            Counter = 0;
            DoorCheckTimer = new System.Timers.Timer(1000);
            DoorCheckTimer.Elapsed += OnTimedDoorCheckEvent;
            DoorCheckTimer.AutoReset = true;
            DoorCheckTimer.Enabled = true;
        }

        private void OnTimedDoorCheckEvent(Object source, ElapsedEventArgs e)
        {
            if (_IO.Read(_checkInModel.LockerSelectedIndex) == _IO.Lock)
            {
                Counter += 1;
                if (Counter == 20)
                {
                    Counter = 0;
                    DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                    DoorCheckTimer.Close();
                    SelectedStepTabName = "操作逾時";

                    _IO.Write(_checkInModel.LockerSelectedIndex, _IO.Lock);
                }
            }
            else
            {
                Counter = 0;
                DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                DoorCheckTimer.Close();

                _IO.Write(_checkInModel.LockerSelectedIndex, _IO.Lock);

                //正常流程
                SetDoorCheckWithRFIDVerifyTimer();
            }

            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";
        }

        private void SetDoorCheckWithRFIDVerifyTimer()
        {
            Counter = 0;
            DoorCheckWithRFIDVerifyTimer = new System.Timers.Timer(1000);
            DoorCheckWithRFIDVerifyTimer.Elapsed += OnTimedDoorCheckWithRFIDVerifyEvent;
            DoorCheckWithRFIDVerifyTimer.AutoReset = true;
            DoorCheckWithRFIDVerifyTimer.Enabled = true;
        }

        private void OnTimedDoorCheckWithRFIDVerifyEvent(Object source, ElapsedEventArgs e)
        {
            if (_IO.Read(_checkInModel.LockerSelectedIndex) == _IO.UnLock)
            {
                Counter += 1;
                if (Counter == 20)
                {
                    Counter = 0;
                    DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                    DoorCheckTimer.Close();
                    SelectedStepTabName = "操作逾時";
                }
            }
            else
            {
                ///此處RFID檢查有無偵測到物件
                bool _rfid = false;
                if (_rfid == true && _IO.Read(_checkInModel.LockerSelectedIndex) == _IO.Lock)
                {
                    Counter = 0;
                    DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                    DoorCheckTimer.Close();

                    NoticeText = "歸還成功，請點擊付款";
                    NextStepIsEnabled = true;
                }
                else
                {
                    Counter = 0;
                    DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                    DoorCheckTimer.Close();

                    SelectedStepTabName = "歸還失敗";
                }
            }

            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";
        }

        private void SetDebitCheckTimer()
        {
            Counter = 0;
            DebitCheckTimer = new System.Timers.Timer(1000);
            DebitCheckTimer.Elapsed += OnTimedDebitCheckTimerEvent;
            DebitCheckTimer.AutoReset = true;
            DebitCheckTimer.Enabled = true;
        }

        private void OnTimedDebitCheckTimerEvent(Object source, ElapsedEventArgs e)
        {
            Counter += 1;
            if (Counter == 20)
            {
                Counter = 0;
                DebitCheckTimer = new System.Timers.Timer(1000);
                DebitCheckTimer.Elapsed += OnTimedDebitCheckTimerEvent;

                SelectedStepTabName = "付款失敗";
            }
            else
            {
                if (_checkInModel.DebitStatus == "成功")
                {
                    Counter = 0;
                    DebitCheckTimer = new System.Timers.Timer(1000);
                    DebitCheckTimer.Elapsed -= OnTimedDebitCheckTimerEvent;

                    SelectedStepTabName = "歸還完成";
                }
            }

            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";
        }

        private async void Charge()
        {
            int _amount = _checkInModel.Amount;
            string _chargeResult = await Task.Run<string>(() => { return _easyCard.Charge_request(_amount); });
            string _isSuccess = (string)JObject.Parse(_chargeResult)["is_success"];
            if (_isSuccess == "true")
            {
                _checkInModel.DebitStatus = "成功";
            }
        }

        public Action FinishInteraction { get; set; }

        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }
    }
}
