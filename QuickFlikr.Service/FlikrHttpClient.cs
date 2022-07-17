using System.Net.Http.Headers;

namespace QuickFlikr.Service
{
    public class FlikrHttpClient : HttpClient
    {
        #region Constructor

        public FlikrHttpClient()
        {
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion Constructor
    }
}