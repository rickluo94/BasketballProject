using First_MVVM.Notifications;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;

namespace First_MVVM.ViewModels
{
    public class RegisterEmailViewModel : BindableBase, IInteractionRequestAware
    {
        public DelegateCommand CancelCommand { get; private set; }
        private string _emailStr;

        public string EmailStr
        {
            get { return _emailStr; }
            set { _emailStr = value; }
        }


        public RegisterEmailViewModel()
        {
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            EmailStr = null;
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
