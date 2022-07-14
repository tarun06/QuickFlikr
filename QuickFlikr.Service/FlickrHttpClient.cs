using System.Net.Http.Headers;

namespace QuickFlikr.Service
{
    public class FlickrHttpClient : HttpClient
    {
        #region Constructor

        public FlickrHttpClient()
        {
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion Constructor
    }
}