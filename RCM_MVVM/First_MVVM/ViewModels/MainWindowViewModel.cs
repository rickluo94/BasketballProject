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

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _passWord;

        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }

        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }


        public DelegateCommand<object> LoginCommand { get; private set; }

        public InteractionRequest<ICustomNotification> RegisterAccountViewRequest { get; set; }
        public InteractionRequest<ICustomNotification> RegisterStepTabRequest { get; set; }
        public InteractionRequest<ICustomNotification> CheckOutStepTabRequest { get; set; }

        public DelegateCommand RegisterAccountViewCommand { get; set; }
        public DelegateCommand RegisterStepTabCommand { get; set; }
        public DelegateCommand CheckOutStepTabCommand { get; set; }

        public MainWindowViewModel()
        {
            IO.SetDevicePort("COM3", 57600); IO.SetIOParameter();
            LoginCommand = new DelegateCommand<object>(LoginExecute);
            RegisterAccountViewRequest = new InteractionRequest<ICustomNotification>();
            RegisterAccountViewCommand = new DelegateCommand(RaiseRegisterAccountView);
            RegisterStepTabRequest = new InteractionRequest<ICustomNotification>();
            RegisterStepTabCommand = new DelegateCommand(RegisterStepTabView);
            CheckOutStepTabRequest = new InteractionRequest<ICustomNotification>();
            CheckOutStepTabCommand = new DelegateCommand(CheckOutStepTabView);
        }

        private void LoginExecute(object parameter) 
        {
            var passwordBox = parameter as PasswordBox;

            if (passwordBox != null) { PassWord = passwordBox.Password; }

            if (!String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(PassWord))
            { 
                string password = "123456";

                if (password == PassWord)
                {
                    UpdateText = "登入成功";
                }
                else
                {
                    UpdateText = "登入失敗";
                }
            }
            else
            {
                UpdateText = "帳號密碼不可為空白";
            }
        }

        private void RaiseRegisterAccountView()
        {
            RegisterAccountViewRequest.Raise(new CustomNotification { Title = "Register Account" });
        }

        private void RegisterStepTabView()
        {
            RegisterStepTabRequest.Raise(new CustomNotification { Title = "Register Account" });
        }

        private void CheckOutStepTabView()
        {
            CheckOutStepTabRequest.Raise(new CustomNotification { Title = " " });
        }
    }
}
