namespace ShecanDesktop.Contracts
{
    public interface ILogger
    {
        void Start(string logFilePath, string dateTimeStamp);
        void LogInfo(string content);
        void LogInfo(string logFilePath, string content);
        void LogError(string content);
        void LogError(string logFilePath, string content);
        void LogDebug(string content);
        void LogDebug(string logFilePath, string content);
    }
}