using Prism.Commands;
using Prism.Mvvm;
using System;
using IOModel;
using Prism.Regions;

namespace First_MVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public static bool CloseLoadingPage = false;

        private string _title = "Smart Locker";
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

        public DelegateCommand<string> NavigateCmd { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            IO.SetDevicePort("COM5", 9600); IO.SetIOParameter();

            _regionManager = regionManager;
            NavigateCmd = new DelegateCommand<string>(Navigate);
        }

        public void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath, NavigationComplete);
        }

        private void NavigationComplete(NavigationResult result)
        {
            //System.Windows.MessageBox.Show(String.Format("Navigation to {0} complete. ", result.Context.Uri));
        }

    }
}
