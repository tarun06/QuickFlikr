using QuickFlikr.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QuickFlikr.WinApp
{
    public class AppUnhandledException
    {
        #region Methods
        public static void HandlingUnhandledExceptions()
        {
            
            // Catch exceptions from all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                ShowUnhandledException(args.ExceptionObject as Exception, ExceptionRes.AppDomainException, true);

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception, ExceptionRes.TaskUnobservedException, true);

            // Catch exceptions from a single specific UI dispatcher thread.
            App.Current.Dispatcher.UnhandledException += (sender, args) =>
            {
                args.Handled = true;
                ShowUnhandledException(args.Exception, ExceptionRes.DispatcherUnhandledException, false);
            };
        }

        private static void ShowUnhandledException(Exception exception, string unhandledExceptionType, bool promptUserForShutdown)
        {
            var messageBoxTitle = string.Format(ExceptionRes.UnExpectedError, unhandledExceptionType);
            var messageBoxMessage = string.Format(ExceptionRes.ExceptionOccurred, exception);
            var messageBoxButtons = MessageBoxButton.OK;

            if (promptUserForShutdown)
            {
                messageBoxMessage += Global.AppWillDie;
                messageBoxButtons = MessageBoxButton.YesNo;
            }

            // Let the user decide if the app should die or not (if applicable).
            if (MessageBox.Show(messageBoxMessage, messageBoxTitle, messageBoxButtons) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion Methods
    }
}