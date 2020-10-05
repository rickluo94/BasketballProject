using First_MVVM.Notifications;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Windows.Controls;

namespace First_MVVM.ViewModels
{
    public class RegisterAccountViewModel : BindableBase, IInteractionRequestAware
    {

        private string _AccountStr;

        public string AccountStr
        {
            get { return _AccountStr; }
            set { SetProperty(ref _AccountStr, value); }
        }

        private string _noticeText;

        public string NoticeText
        {
            get { return _noticeText; }
            set { SetProperty(ref _noticeText, value); }
        }


        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand<object> CheckCommand { get; private set; }
        public InteractionRequest<ICustomNotification> RegisterPasswordViewRequest { get; set; }
        public DelegateCommand RegisterPasswordViewCommand { get; set; }

        public RegisterAccountViewModel()
        {
            CancelCommand = new DelegateCommand(CancelInteraction);
            CheckCommand = new DelegateCommand<object>(CheckAccount);

            RegisterPasswordViewRequest = new InteractionRequest<ICustomNotification>();
            RegisterPasswordViewCommand = new DelegateCommand(RaiseRegisterPasswordView);
        }
        private void CheckAccount(Object parameter)
        {
            var AccountBox = parameter as TextBox;
            if (AccountBox.Text != "123")
            {
                NoticeText = "可以使用";
            }
            else
            {
                NoticeText = "不可以使用";
            }
        }

        private void CancelInteraction()
        {
            AccountStr = null;
            NoticeText = null;
            FinishInteraction?.Invoke();
        }

        private void RaiseRegisterPasswordView()
        {
            if (!string.IsNullOrWhiteSpace(AccountStr))
            {
                RegisterPasswordViewRequest.Raise(new CustomNotification { Title = "Register Password"});
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
