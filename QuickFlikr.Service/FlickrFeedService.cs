using Newtonsoft.Json;
using QuickFlikr.Model;
using QuickFlikr.Resources;
using QuickFlikr.Service.Contract;
using Serilog;

namespace QuickFlikr.Service
{
    public class FlickrFeedService : IFlickrFeedService, IDisposable
    {
        #region Constructor

        public FlickrFeedService(FlickrHttpClient flickrHttpClient, IAppConfiguration appConfiguration)
        {
            if (flickrHttpClient == null)
                throw new ArgumentNullException(string.Format(ExceptionRes.CanNotNull, nameof(FlickrHttpClient)));


            if (appConfiguration == null)
                throw new ArgumentNullException(string.Format(ExceptionRes.CanNotNull, nameof(IAppConfiguration)));

            _flickrHttpClient = flickrHttpClient;
            FlikrServiceUrl = appConfiguration.FlikrServiceUrl;
        }

        #endregion Constructor

        #region Private Feild

        private readonly FlickrHttpClient _flickrHttpClient;
        private static ILogger Logger { get; } = Log.ForContext<FlickrFeedService>();
        private Uri FlikrServiceUrl { get; }

        #endregion Private Feild

        #region Method

        public void Dispose()
        {
            _flickrHttpClient.Dispose();
        }

        public async Task<IEnumerable<FeedInfo>> GetFlickrFeedAsync(string searchInput, CancellationToken cancellationToken = default)
        {
            try
            {
                var feeds = await GetFlikrsAsync(searchInput, Constants.JsonFormat, cancellationToken);
                return feeds.Items;
            }
            catch (OperationCanceledException ex)
            {
                Logger.Error(string.Format(ExceptionRes.CancelledByUser, nameof(GetFlickrFeedAsync)));
                throw new TaskCanceledException(ex.Message);
            }
            catch (HttpRequestException http)
            {
                Logger.Error(string.Format(ExceptionRes.ErrorWhileConnecting, searchInput, http));
                throw new Exception(http.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format(ExceptionRes.ExceptionOnExecuting, ex));
                throw new Exception(ex.Message);
            }
        }

        private async Task<FlickrFeed> GetFlikrsAsync(string searchInput, string format = "json", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(searchInput))
                throw new ArgumentNullException(ExceptionRes.SearchTxtEmpty);

            if (string.IsNullOrEmpty(format))
                throw new ArgumentNullException(ExceptionRes.FormatIsNull);

            Uri.TryCreate(FlikrServiceUrl + $"tags={searchInput}&format={format}&callback=?", UriKind.Absolute, out var uri);

            cancellationToken.ThrowIfCancellationRequested();
            using (HttpResponseMessage response = await _flickrHttpClient.GetAsync(uri, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error(ExceptionRes.FailedResponse);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                // Remove the jsonFlikrFeed()
                result = result.Trim();

                if (result.StartsWith("jsonFlickrFeed("))
                    result = result.Remove(0, "jsonFlickrFeed(".Length);

                if (result.EndsWith(")"))
                    result = result.Substring(0, result.Length - 1);

                return JsonConvert.DeserializeObject<FlickrFeed>(result);
            }
        }

        #endregion Method
    }
}