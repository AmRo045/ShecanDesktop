using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace ShecanDesktop.Core
{
    public class Launcher
    {
        #region Private Members

        private bool _isAppReadyToLaunch;

        #endregion

        #region Public Members

        public void Prepare()
        {
            CheckPrimaryDirectories();
            ManageAppCrash();
            CheckInstances();
            SetAppInfo();
            _isAppReadyToLaunch = true;
        }

        public void Start()
        {
            if (!_isAppReadyToLaunch)
                throw new InvalidOperationException("App is not ready to launch. You must calling the Prepare method first.");

            var mainWindow = new MainWindow();

            Application.Current.MainWindow = mainWindow;
            // Since we register unhandled exceptions
            // manager in App.xaml.cs (EnableUnhandledExceptionManager method) after
            // the application launched, we cannot show the main window using ShowDialog method.
            Application.Current.MainWindow.Show();

            if (!Global.AppInfo.AppCommandLine.Contains("-debug")) return;

            Global.Logger.LogInfo($"Executable Path: {Global.AppInfo.AppFullPath}");
            Global.Logger.LogInfo($"App Version: {Global.AppInfo.AppVersion}");
            Global.Logger.LogInfo($"Command Line: {Global.AppInfo.AppCommandLine}");
            Global.Logger.LogInfo($"Current Os: {Global.GetOsName()}");
        }

        #endregion

        #region Protected Methods

        protected void CheckPrimaryDirectories()
        {
            var primaryDirectories = Global.AppInfo.AppPrimaryDirectories;

            foreach (var primaryDirectory in primaryDirectories)
            {
                if (Directory.Exists(primaryDirectory))
                    continue;

                Directory.CreateDirectory(primaryDirectory);
            }
        }

        protected void ManageAppCrash()
        {
            if (!File.Exists(Global.AppInfo.AppCrashFile))
                return;

            var crashDetails = File.ReadAllLines(Global.AppInfo.AppCrashFile);
            if (crashDetails.Length < 0) return;

            // Crash file new location
            var crashFileNewLocation = $"{Global.AppInfo.AppCrashFolder}{DateTime.Now:yyyy-MM-dd hh-mm-ss}.txt";

            // Prepare crash folder
            if (!Directory.Exists(Global.AppInfo.AppCrashFolder))
                Directory.CreateDirectory(Global.AppInfo.AppCrashFolder);

            // Move crash file to crash folder
            File.Move(Global.AppInfo.AppCrashFile,
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
            Application.Current.Resources["AppVersion"] = Global.AppInfo.AppVersion;
        }

        #endregion
    }
}
