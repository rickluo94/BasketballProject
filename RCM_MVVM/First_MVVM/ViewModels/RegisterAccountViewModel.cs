using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using First_MVVM.Notifications;
using System.Windows.Controls;

namespace First_MVVM.ViewModels
{
    public class RegisterAccountViewModel : BindableBase, IInteractionRequestAware
    {
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand<object> CheckCommand { get; private set; }

        private string _passwordBox;
        public string PasswordBox
        {
            get { return _passwordBox; }
            set { SetProperty(ref _passwordBox, value); }
        }

        public RegisterAccountViewModel()
        {
            CancelCommand = new DelegateCommand(CancelInteraction);
            CheckCommand = new DelegateCommand<object>(CheckPassword);
        }

        private void CheckPassword(object parameter)
       {
            var txtPassword = parameter as TextBox;
            if (txtPassword.Text == "123")
            {
                txtPassword.Text = "456";
            }
        }

        private void CancelInteraction()
        {
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
