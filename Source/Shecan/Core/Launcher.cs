using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Shecan.Common.Log;

namespace Shecan.Core
{
    public class Launcher
    {
        #region Private Members

        private bool _isAppReadyToLaunch;

        #endregion

        #region Static Public Members

        public static string DateTimeStamp => $"{DateTime.Now:yyyy-MM-dd hh-mm-ss}";
        public static ILogger Logger;
        public static LauncherInfo LauncherInfo;

        #endregion

        #region Non-Static Public Members
        
        public virtual void PrepareApplication()
        {
            // Create a instance of LauncherInfo and puts it in LauncherInfo field.
            // We can access to LauncherInfo field from anywhere of the application.
            InitializeLauncher();

            // NOTE: Should be call after LauncherInfo initializing.
            // Because we need to log directory path that will specify in LauncherInfo.
            // Create a logger instance and puts it in Logger field.
            // We can access to Logger field from anywhere of the application.
            StartLogger();

            // NOTE: Should be call after launcher initializing.
            // Because we need to primary directories collection that will specify in LauncherInfo.
            CheckPrimaryDirectories();

            // If powershell script was not found, we have to create it from embedded resources
            CheckPowerShellScript();

            // Manage app crash details
            ManageAppCrash();

            // Make application single instance
            CheckInstances();

            // Set application info such as name, version, etc
            SetAppInfo();

            // Set app status
            _isAppReadyToLaunch = true;
        }

        public virtual void StartApplication()
        {
            if (!_isAppReadyToLaunch)
                throw new InvalidOperationException("App is not ready to launch. You must calling the Prepare method first.");

            var mainWindow = new MainWindow();

            Application.Current.MainWindow = mainWindow;
            // Since we register unhandled exceptions
            // manager in App.xaml.cs (EnableUnhandledExceptionManager method) after
            // the application launched, we cannot show the main window using ShowDialog method.
            Application.Current.MainWindow.Show();
        }

        #endregion

        #region Protected Methods

        protected void InitializeLauncher()
        {
            LauncherInfo = new LauncherInfo();
            LauncherInfo.Initialize();
        }

        protected void StartLogger()
        {
            var logFileName = $"{LauncherInfo.AppLogFolder}" +
                              $"{DateTimeStamp}.log";

            Logger = new Logger();
            Logger.Start(logFileName, DateTimeStamp);
        }

        protected void CheckPrimaryDirectories()
        {
            var primaryDirectories = LauncherInfo.AppPrimaryDirectories;

            foreach (var primaryDirectory in primaryDirectories)
            {
                if (Directory.Exists(primaryDirectory))
                    continue;

                Directory.CreateDirectory(primaryDirectory);
            }
        }

        protected void CheckPowerShellScript()
        {
            if (File.Exists(LauncherInfo.PowerShellScriptFile))
                return;

            try
            {
                var fileName = Path.GetFileName(LauncherInfo.PowerShellScriptFile);
                var resourceName = $"{LauncherInfo.AppNamespace}.Resources.Shell.{fileName}";
                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                using (var file = new FileStream(LauncherInfo.PowerShellScriptFile, FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception.Message);
            }
        }

        protected static void ManageAppCrash()
        {
            if (!File.Exists(LauncherInfo.AppCrashFile))
                return;

            var crashDetails = File.ReadAllLines(LauncherInfo.AppCrashFile);
            if (crashDetails.Length < 0) return;

            // Crash file new location
            var crashFileNewLocation = $"{LauncherInfo.AppCrashFolder}{DateTime.Now:yyyy-MM-dd hh-mm-ss}.txt";

            // Prepare crash folder
            if (!Directory.Exists(LauncherInfo.AppCrashFolder))
                Directory.CreateDirectory(LauncherInfo.AppCrashFolder);

            // Move crash file to crash folder
            File.Move(LauncherInfo.AppCrashFile,
                crashFileNewLocation);

            // Display app last crash details using Notepad
            // TODO: Create a own crash details viewer
            Process.Start("Notepad.exe", crashFileNewLocation);
        }

        protected void CheckInstances()
        {
            var currentProcess = Process.GetCurrentProcess();
            var count = Process.GetProcesses().Count(p => p.ProcessName == currentProcess.ProcessName);
            if (count <= 1) return;
            Environment.Exit(0);
        }

        protected void SetAppInfo()
        {
            Application.Current.Resources["AppVersion"] = LauncherInfo.AppVersion;
        }

        #endregion
    }
}
