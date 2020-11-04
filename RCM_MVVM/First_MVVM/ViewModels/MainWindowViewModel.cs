using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Controls;
using First_MVVM.Models;
using System.Data;
using First_MVVM.Views;
using Prism.Interactivity.InteractionRequest;
using First_MVVM.Notifications;
using IOModel;

namespace First_MVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        public InteractionRequest<ICustomNotification> RegisterStepTabRequest { get; set; }
        public InteractionRequest<ICustomNotification> CheckOutStepTabRequest { get; set; }
        public InteractionRequest<ICustomNotification> CheckInStepTabRequest { get; set; }

        public DelegateCommand RegisterStepTabCommand { get; set; }
        public DelegateCommand CheckOutStepTabCommand { get; set; }
        public DelegateCommand CheckInStepTabCommand { get; set; }

        public MainWindowViewModel()
        {
            //IO.SetDevicePort("COM3", 57600); IO.SetIOParameter();
            RegisterStepTabRequest = new InteractionRequest<ICustomNotification>();
            RegisterStepTabCommand = new DelegateCommand(RegisterStepTabView);
            CheckOutStepTabRequest = new InteractionRequest<ICustomNotification>();
            CheckOutStepTabCommand = new DelegateCommand(CheckOutStepTabView);
            CheckInStepTabRequest = new InteractionRequest<ICustomNotification>();
            CheckInStepTabCommand = new DelegateCommand(CheckInStepTabView);
        }

        private void RegisterStepTabView()
        {
            RegisterStepTabRequest.Raise(new CustomNotification { Title = "Register Account" });
        }

        private void CheckOutStepTabView()
        {
            CheckOutStepTabRequest.Raise(new CustomNotification { Title = " " });
        }

        private void CheckInStepTabView()
        {
            CheckInStepTabRequest.Raise(new CustomNotification { Title = " " });
        }
    }
}
