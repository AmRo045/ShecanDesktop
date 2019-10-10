using System;
using System.IO;
using ShecanDesktop.Contracts;

namespace ShecanDesktop.Common
{
    public class Logger : ILogger
    {
        protected string LogFilePath;
        protected string DateTimeStamp;

        protected virtual void Log(string content, string type)
        {
            if (string.IsNullOrWhiteSpace(LogFilePath))
                throw new InvalidOperationException("You should specify a target file to logging.");

            if (!File.Exists(LogFilePath))
                File.Create(LogFilePath).Dispose();

            var modifiedContent = $"[{DateTimeStamp}] [{type}] {content}";

            using (var logFileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writeStream = new StreamWriter(logFileStream))
                {
                    writeStream.BaseStream.Seek(0L, SeekOrigin.End);
                    writeStream.WriteLine(modifiedContent);
                    writeStream.Flush();
                }
            }
        }

        protected virtual void Log(string logFilePath, string content, string type)
        {
            if (string.IsNullOrWhiteSpace(LogFilePath))
                throw new InvalidOperationException("You should specify a target file to logging.");

            if (!File.Exists(LogFilePath))
                File.Create(LogFilePath).Dispose(); 

            var modifiedContent = $"[{DateTimeStamp}] [{type}] {content}";
            using (var logFileStream = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writeStream = new StreamWriter(logFileStream))
                {
                    writeStream.BaseStream.Seek(0L, SeekOrigin.End);
                    writeStream.WriteLine(modifiedContent);
                    writeStream.Flush();
                }
            }
        }

        public virtual void Start(string logFilePath, string dateTimeStamp)
        {
            LogFilePath = logFilePath;
            DateTimeStamp = dateTimeStamp;
        }

        public virtual void LogInfo(string content)
        {
            const string type = "info";
            Log(content, type);
        }

        public virtual void LogInfo(string logFilePath, string content)
        {
            const string type = "info";
            Log(logFilePath, content, type);
        }

        public virtual void LogError(string content)
        {
            const string type = "error";
            Log(content, type);
        }

        public virtual void LogError(string logFilePath, string content)
        {
            const string type = "error";
            Log(logFilePath, content, type);
        }

        public virtual void LogDebug(string content)
        {
            const string type = "debug";
            Log(content, type);
        }

        public void LogDebug(string logFilePath, string content)
        {
            const string type = "debug";
            Log(logFilePath, content, type);
        }
    }
}