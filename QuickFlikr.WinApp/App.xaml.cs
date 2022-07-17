using Microsoft.Extensions.Configuration;
using QuickFlikr.Service;
using QuickFlikr.Service.Contract;
using System;
using System.Windows;

namespace QuickFlikr.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;


        protected override void OnStartup(StartupEventArgs e)
        {
            AppUnhandledException.HandlingUnhandledExceptions();
            ApplicationBootstrapper.RegisterServices();
            ApplicationBootstrapper.InitializeLogging();
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            QuickFlikrView quickFlikrView = new QuickFlikrView();
            quickFlikrView.DataContext = new QuickFlikrViewModel(
                ApplicationBootstrapper.ServiceLocator.GetService<IFlikrFeedService>());
            quickFlikrView.Show();
        }
    }
}