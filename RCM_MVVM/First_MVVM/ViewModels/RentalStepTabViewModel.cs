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
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace First_MVVM.ViewModels
{
    public class RentalStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        IO _IO = new IO();
        RentalModel _rentalModel = new RentalModel();



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

        private string _lockerSelectedIndex;

        public string LockerSelectedIndex
        {
            get { return _lockerSelectedIndex; }
            set { _lockerSelectedIndex = value; }
        }

        public ObservableCollection<bool> ResStatus { get; private set; } = new ObservableCollection<bool>();
        private Business.ResStatus _resStatusGroup = null;

        #endregion

        #region Interface Command 
        public DelegateCommand<Grid> RentalStepTabLoadCmd { get; private set; }
        public DelegateCommand PreviousTabCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        #endregion

        public RentalStepTabViewModel(Business.ResStatus resStatusGroup)
        {
            _resStatusGroup = resStatusGroup;
            //_IO.SetDevicePort("COM3", 57600); _IO.SetIOParameter();
            RentalStepTabLoadCmd = new DelegateCommand<Grid>(RentalStepTabLoad);
            PreviousTabCmd = new DelegateCommand(PreviousTab);
            NextTabCmd = new DelegateCommand(NextTab);
        }

        private void RentalStepTabLoad(Grid Lockers)
        {
            FillCabinetBtns(Lockers);
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
                case "選擇櫃位":
                    _rentalModel.LockerSelectedIndex = _lockerSelectedIndex;

                    _IO.Write(_rentalModel.LockerSelectedIndex, _IO.UnLock);

                    break;
                case "租借流程":

                    break;
                default:
                    break;
            }
        }

        private void FillCabinetBtns(Grid Lockers)
        {
            //取得櫃位狀態
            List<bool> _resStatuslist = _resStatusGroup.GetAll();
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
            _lockerSelectedIndex = parameter.Content.ToString();
            NextStepIsEnabled = true;
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
