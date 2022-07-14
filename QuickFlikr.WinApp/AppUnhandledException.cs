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
                ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException", true);

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException", true);

            // Catch exceptions from a single specific UI dispatcher thread.
            App.Current.Dispatcher.UnhandledException += (sender, args) =>
            {
                args.Handled = true;
                ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException", false);
            };
        }

        private static void ShowUnhandledException(Exception exception, string unhandledExceptionType, bool promptUserForShutdown)
        {
            var messageBoxTitle = $"Unexpected Error Occurred: {unhandledExceptionType}";
            var messageBoxMessage = $"The following exception occurred:\n\n{exception}";
            var messageBoxButtons = MessageBoxButton.OK;

            if (promptUserForShutdown)
            {
                messageBoxMessage += "\n\nNormally the app would die now. Should we let it die?";
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