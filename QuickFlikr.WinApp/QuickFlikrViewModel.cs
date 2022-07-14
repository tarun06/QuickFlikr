using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickFlikr.Service;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuickFlikr.WinApp
{
    public partial class QuickFlikrViewModel : ObservableObject
    {
        #region Private Field

        private CancellationTokenSource _cancellationTokenSource;
        private IFlickrFeedService _flickerFeedService;
        private bool _isVisible;
        private ObservableCollection<string> _photos = new ObservableCollection<string>();

        #endregion Private Field

        #region Commands

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new RelayCommand<string>(SearchImage));

        #endregion Commands

        #region Methods

        private async Task SearchFeed(string searchText)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                var feedInfos = await _flickerFeedService.GetFlickrFeedAsync(searchText, CancellationToken.None);
                Photos.Clear();
                foreach (var item in feedInfos)
                {
                    Photos.Add(item.Media.Path);
                }
            }
            catch (Exception argEx)
            {
                Logger.Error(argEx.Message);
            }
        }

        private async void SearchImage(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                Photos.Clear();
                return;
            }
            try
            {
                IsVisible = true;
                await SearchFeed(searchText);
            }
            finally
            {
                IsVisible = false;
            }
        }
        #endregion Methods

        #region Properties

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public ObservableCollection<string> Photos
        {
            get => _photos;
            set
            {
                _photos = value;
                OnPropertyChanged(nameof(Photos));
            }
        }

        private static ILogger Logger { get; } = Log.ForContext<QuickFlikrViewModel>();

        #endregion Properties

        #region Constructor

        public QuickFlikrViewModel(IFlickrFeedService flickrFeedService)
        {
            _flickerFeedService = flickrFeedService;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion Constructor
    }
}