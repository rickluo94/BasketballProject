using System;

using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using DBModel;
using IOModel;
using System.Data;
using First_MVVM.Models;
using First_MVVM.Views;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;

namespace First_MVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        LoadingView loadingView;
        private LC_DBRead _lC_DBread = new LC_DBRead();
        private Thread _thread;
        private ManualResetEvent _shutdownEvent;
        private ManualResetEvent _pauseEvent;

        public static Thread _viewerThread;

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

        private bool _checkOutEnabled = true;
        public bool CheckOutEnabled
        {
            get { return _checkOutEnabled; }
            set { SetProperty(ref _checkOutEnabled, value); }
        }

        private bool _availableBox;
        public bool AvailableBox
        {
            get { return _availableBox; }
            set { SetProperty(ref _availableBox, value); }
        }

        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set { SetProperty(ref _loadingVisibility, value); }
        }

        public DelegateCommand<string> NavigateCmd { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCmd = new DelegateCommand<string>(Navigate);

            Start();
        }

        #region SystemConfigLoad

        public void Start()
        {
            _thread = new Thread(SYS_Load);
            _shutdownEvent = new ManualResetEvent(false);
            _pauseEvent = new ManualResetEvent(true);

            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            // trigger stop
            _shutdownEvent.Set();
            // if thread suspend, let it resume.
            _pauseEvent.Set();
            _thread.Join();
            _thread = null;
        }

        #endregion

        private void StartLoad()
        {
            _viewerThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    loadingView = new LoadingView();
                    loadingView.Show();
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception)
                {
                    throw;
                }

            }));

            _viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
            _viewerThread.IsBackground = true;
            _viewerThread.Start();
        }

        private async Task StopLoad()
        {
            Task.Delay(3000).Wait();
            System.Windows.Threading.Dispatcher.FromThread(_viewerThread).InvokeShutdown();
        }

        enum SysSet_SN
        {
            StationName = 1,
        }

        public void SYS_Load()
        {
            LoadingVisibility = Visibility.Visible;
            //系統設定
            System_Set_Load();

            //開啟控制IO;寫入IO初始值;
            IO.SetDevicePort("COM5", 9600); IO.SetIOParameter();
            LoadingVisibility = Visibility.Collapsed;
        }

        public async Task System_Set_Load()
        {
            //載入系統設定值
            DataTable system_set = await _lC_DBread.system_set();
            SystemSetModel.StationName = system_set.Select($"SN='{(int)SysSet_SN.StationName}'")[0]["Value"].ToString();
        }

        public void Navigate(string navigatePath)
        {
            StartLoad();
            if (navigatePath != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath, NavigationComplete);
        }

        private void NavigationComplete(NavigationResult result)
        {
            StopLoad();
            //System.Windows.MessageBox.Show(String.Format("Navigation to {0} complete. ", result.Context.Uri));
        }

    }
}
