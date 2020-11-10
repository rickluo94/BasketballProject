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

namespace First_MVVM.ViewModels
{
    public class MemberServicePageViewModel : BindableBase, IInteractionRequestAware
    {
        private MemberServiceModel _memberServiceModel { get; set; }
        private EasyCard _easyCard = new EasyCard();

        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();

        private System.Timers.Timer PumpStartTimer;

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }

        #region Interface Property

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
        public DelegateCommand MemberServicePageLoadCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }
        public DelegateCommand DebitCmd { get; private set; }
        public DelegateCommand GoToDebitPageCmd { get; private set; }
        public DelegateCommand GoToPumpPageCmd { get; private set; }
        public DelegateCommand PumpBoxStartCmd { get; private set; }

        #endregion

        public MemberServicePageViewModel()
        {
            MemberServicePageLoadCmd = new DelegateCommand(MemberServicePageLoad);
            ExitCmd = new DelegateCommand(ExitInteraction);
            NextTabCmd = new DelegateCommand(NextTab);
            ReadCardCmd = new DelegateCommand(ReadCard);
            GoToDebitPageCmd = new DelegateCommand(GoToDebitPage);
            DebitCmd = new DelegateCommand(Charge);
            GoToPumpPageCmd = new DelegateCommand(GoToPumpPage);
            PumpBoxStartCmd = new DelegateCommand(PumpBoxStart);

        }

        private void MemberServicePageLoad()
        {
            PumpBoxStartIsEnable = true;
            ReadCardIsEnabled = true;
            NextStepIsEnabled = false;
            SelectedStepTabIndex = 0;
            _memberServiceModel = new MemberServiceModel();
            _easyCard.SetDevicePort("COM8", 115200, 500); _easyCard.Open();
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
            FinishInteraction?.Invoke();
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
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            
            Card_id = (string)JObject.Parse(Data)["result"]["card_id"];
            if (string.IsNullOrWhiteSpace(_card_id)) return;
            DataTable _rFID_UsersProfile = await _dBRead.RFID_Users(_card_id);

            if (_rFID_UsersProfile.Rows.Count > 0)
            {
                AccountStr = _rFID_UsersProfile.Rows[0]["RFID_user_id"].ToString();
                BalanceStr = (string)JObject.Parse(Data)["result"]["balance"];

                DataTable _outstanding_Amount = await _dBRead.Charge_History(AccountStr);
                DataTable _checkOut_History = await _dBRead.Take_History(AccountStr);
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
            }
        }

        private void PumpBoxStart()
        {
            IO.Write("A8", IO.UnLock);
            SetPumpStartTimer();
            PumpBoxStartIsEnable = false;
        }


        #region Unsubscribe Event

        private void UnsubscribePumpStartEvent()
        {
            PumpStartTimer.Elapsed -= OnTimePumpStartEvent;
            PumpStartTimer.Close();
        }

        #endregion


        #region Set Subscribe Event
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

        private void OnTimePumpStartEvent(Object source, ElapsedEventArgs e)
        {
            _counter += 1;
            if (IO.Read("A8") == IO.DoorOpen)
            {
                IO.Write("A8", IO.Lock);
            }

            if (IO.Read("Pump") == IO.DoorOpen)
            {
                IO.Write("Pump", IO.Lock);
            }
            else
            {
                IO.Write("Pump", IO.UnLock);
            }

            if (Counter == 20)
            {
                UnsubscribePumpStartEvent();
                IO.Write("A8", IO.Lock);
                IO.Write("Pump", IO.Lock);

                SelectedStepTabName = "完成";
            }
            NoticeText = $"剩餘操作時間 {(20 - Counter)} sec";
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
