using Prism.Commands;
using Prism.Mvvm;
using First_MVVM.Notifications;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;


namespace First_MVVM.ViewModels
{
    public class CheckInStepTabViewModel : BindableBase , IInteractionRequestAware
    {
        #region Interface Property

        private string _noticeText;

        public string NoticeText
        {
            get { return _noticeText; }
            set { _noticeText = value; }
        }


        private bool _nextStepIsEnabled;

        public bool NextStepIsEnabled
        {
            get { return _nextStepIsEnabled; }
            set { _nextStepIsEnabled = value; }
        }

        private string _accountStr;

        public string AccountStr
        {
            get { return _accountStr; }
            set { _accountStr = value; }
        }

        private string _balanceStr;

        public string BalanceStr
        {
            get { return _balanceStr; }
            set { _balanceStr = value; }
        }



        #endregion

        #region Interface Command 
        public DelegateCommand CheckInStepTabLoadCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        public DelegateCommand PreviousTabCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }

        #endregion

        public CheckInStepTabViewModel()
        {
            
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
