using Microsoft.Extensions.Configuration;
using QuickFlikr.Service;
using QuickFlikr.Service.Contract;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace QuickFlikr.WinApp
{
    public static class ApplicationBootstrapper
    {
        #region Properties

        public static readonly ServiceLocator ServiceLocator = new();

        #endregion Properties

        #region Methods

        public static IConfiguration BuildConfiguration()
        {
            var systemFolders = ServiceLocator.GetService<ISystemFolders>();
            IConfigurationBuilder builder = new ConfigurationBuilder();
            foreach (var folder in systemFolders.ConfigurationFolders)
            {
                var filePath = GetEnvConfig(folder);
                if (!File.Exists(filePath)) continue;
                builder = builder
                  // Load base config file for all environments.
                  .AddJsonFile(filePath, optional: false, reloadOnChange: false);
            }
            return builder.Build();

            string GetEnvConfig(string folder)
            {
                return Path.Combine(folder, "appsettings.json");
            }
        }

        public static void InitializeLogging()
        {
            var config = ServiceLocator.GetService<IConfiguration>();
            if (!Enum.TryParse<LogEventLevel>(config["Serilog:MinimumLevel:Default"], out var logLevel))
            {
                logLevel = LogEventLevel.Information;
            }

            var systemFolder = ServiceLocator.GetService<ISystemFolders>();
            var path = Path.Combine(systemFolder.LogsFolder, $"Logs{DateTime.Today.ToString("yyyyMMdd")}.json");
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Is(logLevel)
                .WriteTo.File(path)
                .CreateLogger();
        }

        public static void RegisterServices()
        {
            ServiceLocator.RegisterSingleton<IFlickrFeedService, FlickrFeedService>();
            ServiceLocator.RegisterSingleton<ISystemFolders, SystemFolders>();
            ServiceLocator.RegisterSingleton(BuildConfiguration);
            ServiceLocator.RegisterSingleton<IAppConfiguration, AppConfiguration>();
            ServiceLocator.Register<FlickrHttpClient>();
        }

        #endregion Methods
    }
}