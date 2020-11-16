using First_MVVM.Notifications;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;

namespace First_MVVM.ViewModels
{
    public class LoadingPageViewModel : BindableBase, IInteractionRequestAware
    {
        public DelegateCommand<Storyboard> LoadingPageLoadCmd { get; private set; }

        public LoadingPageViewModel()
        {
            LoadingPageLoadCmd = new DelegateCommand<Storyboard>(LoadingPageLoad);
        }

        private void LoadingPageLoad(Storyboard _basketballSB)
        {
            _basketballSB.Begin();
        }

        #region MainWindow Interactive

        public Action FinishInteraction { get; set; }

        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }

        #endregion
    }
}
