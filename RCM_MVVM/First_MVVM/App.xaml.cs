using Prism.Modularity;
using Prism.Unity;
using Prism.Ioc;
using First_MVVM.Views;
using First_MVVM.Views.RegisterStep;
using System.Windows;
using Prism.Mvvm;
using First_MVVM.ViewModels;

namespace First_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
            ViewModelLocationProvider.Register<RegisterAccountView, RegisterAccountViewModel>();
            ViewModelLocationProvider.Register<RegisterPasswordView, RegisterPasswordViewModel>();
            ViewModelLocationProvider.Register<RegisterEmailView, RegisterEmailViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleA.ModuleAModule>();
        }
    }
}
