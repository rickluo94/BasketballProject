using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using First_MVVM.Notifications;
using System.Windows.Controls;
using System.Diagnostics;
using First_MVVM.Models;

namespace First_MVVM.ViewModels
{
    public class RegisterPasswordViewModel : BindableBase, IInteractionRequestAware
    {
        RegisterModel RegisterModel = new RegisterModel();
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand<object> ClearPasswordCommand { get; private set; }
        public DelegateCommand<object> CheckCommand { get; private set; }
        public DelegateCommand<object> ConfirmCommand { get; private set; }

        private string _noticeText;

        public string NoticeText
        {
            get { return _noticeText; }
            set { SetProperty(ref _noticeText, value); }
        }


        private string _passwordStr;
        public string PasswordStr
        {
            get { return _passwordStr; }
            set { SetProperty(ref _passwordStr, value); }
        }

        private string _confirmPasswordStr;

        public string ConfirmPasswordStr
        {
            get { return _confirmPasswordStr; }
            set { SetProperty(ref _confirmPasswordStr, value); }
        }


        public RegisterPasswordViewModel()
        {
            CancelCommand = new DelegateCommand(CancelInteraction);
            CheckCommand = new DelegateCommand<object>(CheckPassword);
            ConfirmCommand = new DelegateCommand<object>(ConfirmPassword);
            ClearPasswordCommand = new DelegateCommand<object>(ClearPassword);
        }
        private void ClearPassword(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            passwordBox.Clear();
        }

        private void CheckPassword(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (RegisterModel.Checkpassword(passwordBox.Password))
            {
                PasswordStr = passwordBox.Password;
                if (ConfirmPasswordStr == PasswordStr)
                {
                    NoticeText = "可用";
                }
                else
                {
                    NoticeText = "密碼不一致";
                }
            }
            else
            {
                passwordBox.Clear();
            }
        }
        private void ConfirmPassword(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (RegisterModel.Checkpassword(passwordBox.Password))
            {
                ConfirmPasswordStr = passwordBox.Password;
                if (passwordBox.Password == PasswordStr)
                {
                    NoticeText = "可用";
                }
                else
                {
                    NoticeText = "密碼不一致";
                }
            }
            else
            {
                passwordBox.Clear();
            }
        }
        private void CancelInteraction()
        {
            NoticeText = null;
            PasswordStr = null;
            FinishInteraction?.Invoke();
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
