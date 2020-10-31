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
using System.Diagnostics;

namespace First_MVVM.ViewModels
{
    public class RentalStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        private RentalModel _rentalModel { get; set; }
        private IO _IO = new IO();
        private EasyCard _easyCard = new EasyCard();
        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();
        private System.Timers.Timer OperationTimer;
        private System.Timers.Timer DoorCheckTimer;

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
        private Business.ResStatus _resStatusGroup = null;
        private List<bool> _resStatuslist { get;  set; }

        #endregion

        #region Interface Command 
        public DelegateCommand<Grid> RentalStepTabLoadCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        public DelegateCommand PreviousTabCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }

        #endregion

        public RentalStepTabViewModel(Business.ResStatus resStatusGroup)
        {
            _rentalModel = new RentalModel();
            _resStatusGroup = resStatusGroup;
            //_IO.SetDevicePort("COM3", 57600); _IO.SetIOParameter();
            _easyCard.SetDevicePort("COM6", 115200, 500); _easyCard.Open();
            RentalStepTabLoadCmd = new DelegateCommand<Grid>(RentalStepTabLoad);
            ExitCmd = new DelegateCommand(ExitInteraction);
            PreviousTabCmd = new DelegateCommand(PreviousTab);
            NextTabCmd = new DelegateCommand(NextTab);
            ReadCardCmd = new DelegateCommand(ReadCard);

        }

        private void RentalStepTabLoad(Grid Lockers)
        {
            NextStepIsEnabled = true;
            SelectedStepTabIndex = 0;
            if (CheckAvailableUse() == true) { FillCabinetBtns(Lockers); } else { MessageBox.Show("目前沒有可租借籃球"); ExitInteraction(); }
        }

        private bool CheckAvailableUse()
        {
            //Get res status
            _resStatuslist = _resStatusGroup.GetAll();
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
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            string _card_id = (string)JObject.Parse(Data)["result"]["card_id"];
            if (string.IsNullOrWhiteSpace(_card_id)) return;
            DataTable table = await _dBRead.Verify_TheMember(_card_id);
            if (table.Rows.Count > 0)
            {
                AccountStr = table.Rows[0]["UserID"].ToString();
                BalanceStr = (string)JObject.Parse(Data)["result"]["balance"];
                NoticeText = string.Empty;
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

        private void NextTab()
        {
            FillProfile();
            NextStepIsEnabled = false;
            SelectedStepTabIndex += 1;
        }

        private void PreviousTab()
        {
            SelectedStepTabIndex -= 1;
        }

        private void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "登入":
                    _rentalModel.ID = _accountStr;
                    _rentalModel.Balance = _balanceStr;

                    SetOperationTimer();
                    break;
                case "選擇櫃位":
                    _rentalModel.LockerSelectedIndex = _lockerSelectedIndex;
                    //_IO.Write(_rentalModel.LockerSelectedIndex, _IO.UnLock);
                    NoticeText = "提醒您開啟球櫃後開始計費";
                    SetDoorCheckTimer();
                    break;
                case "租借完成":
                    

                    break;
                default:
                    break;
            }
        }

        private void FillCabinetBtns(Grid Lockers)
        {
            Lockers.Children.Clear();
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
                CabinetButton[i].Width = 200;
                CabinetButton[i].Height = 200;
                CabinetButton[i].IsEnabled = _resStatuslist[i];
                CabinetButton[i].Margin = new Thickness() { Bottom = 10, Left = 10, Right = 10, Top = 10 };
                if (i < 4)
                {
                    Grid.SetColumn(CabinetButton[i], 1);
                    Grid.SetRow(CabinetButton[i], i);
                }
                else
                {
                    Grid.SetColumn(CabinetButton[i], 2);
                    Grid.SetRow(CabinetButton[i], i - 4);
                }
                CabinetButton[i].Click += new System.Windows.RoutedEventHandler(CabinetBtns_Click);
                Lockers.Children.Add(CabinetButton[i]);
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
            FinishInteraction?.Invoke();
        }

        private void SetOperationTimer()
        {
            Counter = 0;
            OperationTimer = new System.Timers.Timer(1000);
            OperationTimer.Elapsed += OnTimedOperationEvent;
            OperationTimer.AutoReset = true;
            OperationTimer.Enabled = true;
        }

        private void OnTimedOperationEvent(Object source, ElapsedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_rentalModel.LockerSelectedIndex))
            {
                Counter += 1;
                if (Counter == 5)
                {
                    Counter = 0;
                    OperationTimer.Elapsed -= OnTimedOperationEvent;
                    OperationTimer.Close();
                    SelectedStepTabName = "操作逾時";
                }
            }
            else
            {
                Counter = 0;
                OperationTimer.Elapsed -= OnTimedOperationEvent;
                OperationTimer.Close();
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
            if (_IO.Read(_rentalModel.LockerSelectedIndex) == _IO.Lock)
            {
                Counter += 1;
                if (Counter == 10)
                {
                    Counter = 0;
                    DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                    DoorCheckTimer.Close();
                    SelectedStepTabName = "操作逾時";
                    _IO.Write(_rentalModel.LockerSelectedIndex, _IO.Lock);
                }
            }
            else
            {
                Counter = 0;
                DoorCheckTimer.Elapsed -= OnTimedDoorCheckEvent;
                DoorCheckTimer.Close();
                _IO.Write(_rentalModel.LockerSelectedIndex, _IO.Lock);

                ///資料庫新增會員借出紀錄依據_rentalModel內資料寫入//

                SelectedStepTabName = "租借完成";
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
