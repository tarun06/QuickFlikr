namespace QuickFlikr.Service.Contract
{
    public interface ISystemFolders
    {
        IEnumerable<string> ConfigurationFolders { get; }

        string LogsFolder { get; }
    }
}