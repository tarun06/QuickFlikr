using QuickFlikr.Service.Contract;

namespace QuickFlikr.Service
{
    public sealed class SystemFolders : ISystemFolders
    {
        #region Methods

        private static string GetAppDataFolder()
        {
            var localAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(localAppdata, SystemRootFolderName);
        }

        private static string GetSystemRootFolder()
        {
            var pathRoot = Path.GetPathRoot(Environment.SystemDirectory);
            if (pathRoot != null
                && Directory.Exists(Path.Combine(pathRoot, SystemRootFolderName)))
            {
                return Path.Combine(pathRoot, SystemRootFolderName);
            }

            return null;
        }

        #endregion Methods

        #region Properties

        public IEnumerable<string> ConfigurationFolders => new[]
        {
            _baseFolder,
            _appDataFolder,
            _systemRootFolder
        }.Where(f => f != null)!;

        public string LogsFolder => GetAppDataFolder();

        #endregion Properties

        #region Private Fields

        private const string SystemRootFolderName = "QuickFlikr";
        private readonly string _appDataFolder = GetAppDataFolder();
        private readonly string _baseFolder = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _systemRootFolder = GetSystemRootFolder();

        #endregion Private Fields
    }
}