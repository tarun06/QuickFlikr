using Newtonsoft.Json;
using QuickFlikr.Model;
using QuickFlikr.Service.Contract;
using Serilog;

namespace QuickFlikr.Service
{
    public class FlickrFeedService : IFlickrFeedService, IDisposable
    {
        #region Constructor

        public FlickrFeedService(FlickrHttpClient flickrHttpClient, IAppConfiguration appConfiguration)
        {
            _flickrHttpClient = flickrHttpClient;
            FlikrServiceUrl = appConfiguration.FlikrServiceUrl;
        }

        #endregion Constructor

        #region Private Feild

        private const string DataFormat = "json";
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
                var feeds = await GetFlikrsAsync(searchInput, DataFormat, cancellationToken);
                return feeds.Items;
            }
            catch (OperationCanceledException ex)
            {
                Logger.Error("GetFlickrFeedAsync is cancelled by user");
                throw new TaskCanceledException(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error($"An exception has occured while executing GetFlickrFeedAsync :- {ex}.");
                throw new Exception(ex.Message);
            }
        }

        private async Task<FlickrFeed> GetFlikrsAsync(string searchInput, string format = "json", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(searchInput))
                throw new ArgumentNullException("Search text can not be null or empty");

            if (string.IsNullOrEmpty(format))
                throw new ArgumentNullException("format can not be null or empty");

            Uri.TryCreate(FlikrServiceUrl + $"tags={searchInput}&format={format}&callback=?", UriKind.Absolute, out var uri);

            cancellationToken.ThrowIfCancellationRequested();
            using (HttpResponseMessage response = await _flickrHttpClient.GetAsync(uri, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error("Failed to get the response from external service.");
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