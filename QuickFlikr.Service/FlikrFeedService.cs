using Newtonsoft.Json;
using QuickFlikr.Model;
using QuickFlikr.Resources;
using QuickFlikr.Service.Contract;
using Serilog;

namespace QuickFlikr.Service
{
    public class FlikrFeedService : IFlikrFeedService, IDisposable
    {
        #region Constructor

        public FlikrFeedService(FlikrHttpClient flikrHttpClient, IAppConfiguration appConfiguration)
        {
            if (flikrHttpClient == null)
                throw new ArgumentNullException(string.Format(Global.CanNotNull, nameof(FlikrHttpClient)));


            if (appConfiguration == null)
                throw new ArgumentNullException(string.Format(Global.CanNotNull, nameof(IAppConfiguration)));

            _flikrHttpClient = flikrHttpClient;
            FlikrServiceUrl = appConfiguration.FlikrServiceUrl;
        }

        #endregion Constructor

        #region Private Feild

        private readonly FlikrHttpClient _flikrHttpClient;
        private static ILogger Logger { get; } = Log.ForContext<FlikrFeedService>();
        private Uri FlikrServiceUrl { get; }

        #endregion Private Feild

        #region Method

        public void Dispose()
        {
            _flikrHttpClient.Dispose();
        }

        /// <summary>
        /// Get Flikr Feed based on searchInput
        /// </summary>
        /// <param name="searchInput"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<FeedInfo>> GetFlikrFeedAsync(string searchInput, CancellationToken cancellationToken = default)
        {
            try
            {
                var feeds = await GetFlikrsAsync(searchInput, Constants.JsonFormat, cancellationToken);
                return feeds.Items;
            }
            catch (OperationCanceledException ex)
            {
                Logger.Error(string.Format(Global.CancelledByUser, nameof(GetFlikrFeedAsync)));
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

        /// <summary>
        /// Get Fliker feed based on searchInput
        /// </summary>
        /// <param name="searchInput"></param>
        /// <param name="format"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<FlikrFeed> GetFlikrsAsync(string searchInput, string format = "json", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(searchInput))
                throw new ArgumentNullException(Global.SearchTxtEmpty);

            if (string.IsNullOrEmpty(format))
                throw new ArgumentNullException(Global.FormatIsNull);

            Uri.TryCreate(FlikrServiceUrl + $"tags={searchInput}&format={format}&callback=?", UriKind.Absolute, out var uri);

            cancellationToken.ThrowIfCancellationRequested();
            using (HttpResponseMessage response = await _flikrHttpClient.GetAsync(uri, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error(Global.FailedResponse);
                    return null;
                }

                cancellationToken.ThrowIfCancellationRequested();
                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                result = result.Trim();

                // Remove the jsonFlickrFeed()
                if (result.StartsWith("jsonFlickrFeed("))
                    result = result.Remove(0, "jsonFlickrFeed(".Length);

                if (result.EndsWith(")"))
                    result = result.Substring(0, result.Length - 1);

                cancellationToken.ThrowIfCancellationRequested();
                return JsonConvert.DeserializeObject<FlikrFeed>(result);
            }
        }

        #endregion Method
    }
}