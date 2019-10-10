using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ShecanDesktop.Core
{
    public class AppInfo
    {
        /// <summary>
        ///     Setting up the <see cref="AppInfo" /> properties value
        /// </summary>
        public void Initialize()
        {
            SetAppCommandLine();
            SetAppName();
            SetAppNameWithoutExe();
            SetAppFullPath();
            // Must call after SetAppFullPath() method
            SetAppRootFolder();
            // Must call after SetAppFullPath() method
            SetAppVersion();
            SetAppFrameworkVersion();
            SetAppLogFolder();
            SetAppCrashFolder();
            SetAppConfigFile();
            SetAppPrimaryDirectories();
            SetAppNamespace();
            SetAppCrashFile();
        }


        /// <summary>
        ///     Get the application command line as a row <see cref="string" />
        /// </summary>
        public string AppCommandLine { get; private set; }

        /// <summary>
        ///     Get the application name (such as app.exe)
        /// </summary>
        public string AppName { get; private set; }

        /// <summary>
        ///     Get the application name without extension (such as app)
        /// </summary>
        public string AppNameWithoutExe { get; private set; }

        /// <summary>
        ///     Get the application full path
        /// </summary>
        public string AppFullPath { get; private set; }

        /// <summary>
        ///     Get the application root folder path
        /// </summary>
        public string AppRootFolder { get; private set; }

        /// <summary>
        ///     Get the application version
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        ///     Get the application framework version
        /// </summary>
        public string AppFrameworkVersion { get; private set; }

        /// <summary>
        ///     Get the application log folder path
        /// </summary>
        public string AppLogFolder { get; private set; }

        /// <summary>
        ///     Get the application crash folder path
        /// </summary>
        public string AppCrashFolder { get; private set; }

        /// <summary>
        ///     Get the application crash file path
        /// </summary>
        public string AppCrashFile { get; private set; }

        /// <summary>
        ///     Get the application config file path
        /// </summary>
        public string AppConfigFile { get; private set; }

        /// <summary>
        ///     Get the application primary directories
        /// </summary>
        public IEnumerable<string> AppPrimaryDirectories { get; set; }

        /// <summary>
        ///     Get the application namespace
        /// </summary>
        public string AppNamespace { get; private set; }


        /// <summary>
        ///     Set application command line arguments
        /// </summary>
        protected void SetAppCommandLine()
        {
            var commandLine = Environment.GetCommandLineArgs()
                .Skip(1) // Remove app path
                .ToList();

            AppCommandLine = string.Join(",", commandLine);
        }

        /// <summary>
        ///     Set application name
        /// </summary>
        protected void SetAppName()
        {
            AppName = AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        ///     Set application name without extension
        /// </summary>
        protected void SetAppNameWithoutExe()
        {
            AppNameWithoutExe = Process.GetCurrentProcess().ProcessName;
        }

        /// <summary>
        ///     Set application full path
        /// </summary>
        protected void SetAppFullPath()
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
                AppFullPath = processModule.FileName.Replace(".vshost", string.Empty);
        }

        /// <summary>
        ///     Set application root folder
        /// </summary>
        protected void SetAppRootFolder()
        {
            // Extract app folder path from it full path
            AppRootFolder = Path.GetDirectoryName(AppFullPath) + "\\";
        }

        /// <summary>
        ///     Set application semantic version
        /// </summary>
        protected void SetAppVersion()
        {
            // Get main assembly version info
            var versionInfo = FileVersionInfo.GetVersionInfo(AppFullPath);
            // Create semantic version of main assembly file version
            var semanticVersion = $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";

            AppVersion = semanticVersion;
        }

        /// <summary>
        ///     Set application framework version
        /// </summary>
        protected void SetAppFrameworkVersion()
        {
            AppFrameworkVersion = Environment.Version.ToString();
        }

        /// <summary>
        ///     Set the application log folder path
        /// </summary>
        protected void SetAppLogFolder()
        {
            AppLogFolder = $"{AppRootFolder}Logs\\";
        }

        /// <summary>
        ///     Set the application crash folder path
        /// </summary>
        protected void SetAppCrashFolder()
        {
            AppCrashFolder = $"{AppRootFolder}CrashReports\\";
        }

        /// <summary>
        ///     Set the application crash file path
        /// </summary>
        protected void SetAppCrashFile()
        {
            AppCrashFile = $"{AppRootFolder}.crash";
        }

        /// <summary>
        ///     Set the application config file path
        /// </summary>
        protected void SetAppConfigFile()
        {
            AppConfigFile = $"{AppRootFolder}.config";
        }

        /// <summary>
        ///     Set the application primary directories
        /// </summary>
        protected void SetAppPrimaryDirectories()
        {
            AppPrimaryDirectories = new List<string>
            {
                AppLogFolder,
                AppCrashFolder
            };
        }

        /// <summary>
        ///     Set the application namespace
        /// </summary>
        protected void SetAppNamespace()
        {
            var appType = typeof(App);
            AppNamespace = appType.Namespace;
        }
    }
}