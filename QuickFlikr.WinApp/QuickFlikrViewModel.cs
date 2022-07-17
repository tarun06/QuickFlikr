using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickFlikr.Resources;
using QuickFlikr.Service;
using Serilog;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Threading.Tasks;

namespace QuickFlikr.WinApp
{
    public partial class QuickFlikrViewModel : ObservableObject
    {
        #region Private Field
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource _taskCompletion;
        private IFlikrFeedService _flikerFeedService;
        private bool _inProgress;
        private string _errorText = string.Empty;
        private ObservableCollection<string> _photos = new ObservableCollection<string>();
        #endregion Private Field

        #region Commands

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new RelayCommand<string>(SearchImage));

        #endregion Commands

        #region Methods

        private async void SearchImage(string searchText)
        {
            ErrorText = string.IsNullOrEmpty(searchText) ? Global.NoInput : string.Empty;
            if (string.IsNullOrEmpty(searchText))
            {
                Photos.Clear();
                return;
            }

            // A taskCompletion is monitor to wait for previous tasks
            if (_taskCompletion is not null)
            {
                // Cancelled previous task if new request is raised.
                _cancellationTokenSource.Cancel();

                // Wait for previous task to complete
                await _taskCompletion.Task;
            }

            _taskCompletion = new TaskCompletionSource();

            try
            {
                Photos.Clear();

                // Set CancellationSource
                SetCancellation();

                SetInProgress(true);

                var feedInfos = await _flikerFeedService.GetFlikrFeedAsync(searchText, _cancellationTokenSource.Token);

                foreach (var item in feedInfos)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    Photos.Add(item.Media.Path);
                }
            }
            catch (OperationCanceledException Oce)
            {
                Photos.Clear();
                Logger.Error(Oce.Message);
            }
            catch (Exception argEx)
            {
                Logger.Error(argEx.Message);
            }
            finally
            {
                SetInProgress(false);
                var isPreviousTaskCancelled = _cancellationTokenSource.IsCancellationRequested;

                // Set error text if previous task is not cancelled and no photograph is received.
                ErrorText = !Photos.Any() && !isPreviousTaskCancelled ? Global.NoPhotoFound : string.Empty;

                // Set Completion to release
                _taskCompletion?.TrySetResult();
            }
        }

        private void SetCancellation()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.Token.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();
        }

        private void SetInProgress(bool inProgress)
        {
            InProgress = inProgress;
        }

        #endregion Methods

        #region Properties

        public string ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }

        public bool InProgress
        {
            get => _inProgress;
            set => SetProperty(ref _inProgress, value);
        }
        public ObservableCollection<string> Photos
        {
            get => _photos;
            set => SetProperty(ref _photos, value);
        }

        private static ILogger Logger { get; } = Log.ForContext<QuickFlikrViewModel>();

        #endregion Properties

        #region Constructor

        public QuickFlikrViewModel(IFlikrFeedService flikrFeedService)
        {
            if (flikrFeedService == null)
                throw new ArgumentNullException(string.Format(Global.CanNotNull, nameof(IFlikrFeedService)));

            _flikerFeedService = flikrFeedService;
        }

        #endregion Constructor
    }
}