using Microsoft.Extensions.Configuration;
using QuickFlikr.Service.Contract;

namespace QuickFlikr.Service
{
    public class AppConfiguration : IAppConfiguration
    {
        private readonly IConfiguration _configuration;

        public AppConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("Configuration can not be null");

            _configuration = configuration;

            FlikrServiceUrl = GetRequiredService("Endpoints:FlikrServiceUrl");
        }

        public Uri FlikrServiceUrl { get; private set; }

        private Uri GetRequiredService(string key)
        {
            return Uri.TryCreate(_configuration[key], UriKind.Absolute, out var uri) ? uri : null;
        }
    }
}