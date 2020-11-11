using Prism.Ioc;
using Prism.Unity;
using System;
using System.Windows;
using First_MVVM.Views;
using Prism.Modularity;

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
            containerRegistry.Register<Business.BoxItemStatus, Business.DBBoxItemStatus>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
        
        }
    }
}
