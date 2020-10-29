using Prism.Commands;
using First_MVVM.Notifications;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using IOModel;
using First_MVVM.Models;
using First_MVVM.Business;

namespace First_MVVM.ViewModels
{
    public class RentalStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        IO _IO = new IO();
        RentalModel _rentalModel = new RentalModel();

        ResStatus _resStatus = new ResStatus();

        #region Interface Property
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

        private int _lockerSelectedIndex;

        public int LockerSelectedIndex
        {
            get { return _lockerSelectedIndex; }
            set { _lockerSelectedIndex = value; }
        }

        private bool _a1_IsEnabled;

        public bool A1_IsEnabled
        {
            get { return _a1_IsEnabled; }
            set { SetProperty(ref _a1_IsEnabled, value); }
        }

        private bool _a2_IsEnabled;

        public bool A2_IsEnabled
        {
            get { return _a2_IsEnabled; }
            set { SetProperty(ref _a2_IsEnabled, value); }
        }

        private bool _a3_IsEnabled;

        public bool A3_IsEnabled
        {
            get { return _a3_IsEnabled; }
            set { SetProperty(ref _a3_IsEnabled, value); }
        }

        private bool _a4_IsEnabled;

        public bool A4_IsEnabled
        {
            get { return _a4_IsEnabled; }
            set { SetProperty(ref _a4_IsEnabled, value); }
        }

        private bool _a5_IsEnabled;

        public bool A5_IsEnabled
        {
            get { return _a5_IsEnabled; }
            set { SetProperty(ref _a5_IsEnabled, value); }
        }

        private bool _a6_IsEnabled;

        public bool A6_IsEnabled
        {
            get { return _a6_IsEnabled; }
            set { SetProperty(ref _a6_IsEnabled, value); }
        }

        private bool _a7_IsEnabled;

        public bool A7_IsEnabled
        {
            get { return _a7_IsEnabled; }
            set { SetProperty(ref _a7_IsEnabled, value); }
        }



        #endregion

        #region Interface Command 
        public DelegateCommand RentalStepTabLoadCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand A1_Cmd { get; private set; }
        public DelegateCommand A2_Cmd { get; private set; }
        public DelegateCommand A3_Cmd { get; private set; }
        public DelegateCommand A4_Cmd { get; private set; }
        public DelegateCommand A5_Cmd { get; private set; }
        public DelegateCommand A6_Cmd { get; private set; }
        public DelegateCommand A7_Cmd { get; private set; }
        #endregion

        public RentalStepTabViewModel()
        {
            //_IO.SetDevicePort("COM3", 57600); _IO.SetIOParameter();
            RentalStepTabLoadCmd = new DelegateCommand(RentalStepTabLoad);
            NextTabCmd = new DelegateCommand(NextTab);
            A1_Cmd = new DelegateCommand(A1_Selected);
            A2_Cmd = new DelegateCommand(A2_Selected);
            A3_Cmd = new DelegateCommand(A3_Selected);
            A4_Cmd = new DelegateCommand(A4_Selected);
            A5_Cmd = new DelegateCommand(A5_Selected);
            A6_Cmd = new DelegateCommand(A6_Selected);
            A7_Cmd = new DelegateCommand(A7_Selected);
        }

        private void RentalStepTabLoad()
        {
            #region 更新取得物品狀態
            if (_resStatus.Update() == true)
            {
                A1_IsEnabled = _resStatus.A1;
                A2_IsEnabled = _resStatus.A2;
                A3_IsEnabled = _resStatus.A3;
                A4_IsEnabled = _resStatus.A4;
                A5_IsEnabled = _resStatus.A5;
                A6_IsEnabled = _resStatus.A6;
                A7_IsEnabled = _resStatus.A7;
            }
            else
            {
                
            }
            #endregion
        }

        private void NextTab()
        {
            FillProfile();
            NextStepIsEnabled = false;
            SelectedStepTabIndex += 1;
        }

        private void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "選擇櫃位":
                    _rentalModel.LockerName = _lockerSelectedIndex;
                    break;
                case "租借流程":
                    
                    break;
                default:
                    break;
            }
        }

        private void A1_Selected() { LockerSelectedIndex = 1; }
        private void A2_Selected() { LockerSelectedIndex = 2; }
        private void A3_Selected() { LockerSelectedIndex = 3; }
        private void A4_Selected() { LockerSelectedIndex = 4; }
        private void A5_Selected() { LockerSelectedIndex = 5; }
        private void A6_Selected() { LockerSelectedIndex = 6; }
        private void A7_Selected() { LockerSelectedIndex = 7; }
        

        public Action FinishInteraction { get; set; }

        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }
    }
}
