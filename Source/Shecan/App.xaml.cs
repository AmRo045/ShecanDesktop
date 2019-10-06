using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Shecan.Core;

namespace Shecan
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var launcher = new Launcher();
            launcher.PrepareApplication();
            launcher.StartApplication();

            // Must call after the launcher.PrepareApplication() method
            EnableUnhandledExceptionManager();

            base.OnStartup(e);
        }

        private void EnableUnhandledExceptionManager()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                LogException((Exception)args.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            if (Dispatcher != null)
            {
                Dispatcher.UnhandledException += (sender, args) =>
                {
                    LogException(args.Exception, "Dispatcher.UnhandledException");
                };
            }

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                LogException(args.Exception, "TaskScheduler.UnobservedTaskException");
            };
        }

        private static void LogException(Exception exception, string type)
        {
            // Get stack trace for the exception with source file information
            var stackTrace = new StackTrace(exception, true);

            // Get the top stack frame
            var frame = stackTrace.GetFrame(0);

            // Get current assembly info
            var assemblyName = Assembly.GetExecutingAssembly().GetName();

            // Log exception details
            using (var crashWriter = new StreamWriter(Launcher.LauncherInfo.AppCrashFile))
            {
                crashWriter.BaseStream.Seek(0L, SeekOrigin.End);
                crashWriter.Flush();

                crashWriter.WriteLine($"Unhandled exception occurred in {assemblyName.Name} v{assemblyName.Version}");
                crashWriter.WriteLine($"At                : {Launcher.DateTimeStamp}");
                crashWriter.WriteLine($"Type              : {type}");
                crashWriter.WriteLine($"File name         : {Path.GetFileName(frame.GetFileName())}");
                crashWriter.WriteLine($"Method name       : {frame.GetMethod().Name}");
                crashWriter.WriteLine($"Line number       : {frame.GetFileLineNumber()}");
                crashWriter.WriteLine($"Exception message : {exception.Message}");

                if (exception.InnerException != null)
                    crashWriter.WriteLine($"Inner message     : {exception.InnerException.Message}");

                crashWriter.WriteLine("--------------------------------Exception Details--------------------------------");
                crashWriter.WriteLine();
                crashWriter.WriteLine(exception);
            }
        }
    }
}
