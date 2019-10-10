using System;
using Microsoft.Win32;
using ShecanDesktop.Common;
using ShecanDesktop.Contracts;

namespace ShecanDesktop.Core
{
    public class Global
    {
        public static string DateTimeStamp;
        public static AppInfo AppInfo;
        public static ILogger Logger;


        public static void Initialize()
        {
            DateTimeStamp = $"{DateTime.Now:yyyy-MM-dd hh-mm-ss}";

            PrepareAppInfo();
            StartLogger();
        }

        public static string GetOsName()
        {
            const string systemInfoPathInRegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\";
            return Registry.GetValue(systemInfoPathInRegistry, "ProductName", null).ToString();
        }

        private static void PrepareAppInfo()
        {
            AppInfo = new AppInfo();
            AppInfo.Initialize();
        }

        private static void StartLogger()
        {
            var logFileName = $"{AppInfo.AppLogFolder}" +
                              $"{DateTimeStamp}.log";

            Logger = new Logger();
            Logger.Start(logFileName, DateTimeStamp);
        }
    }
}