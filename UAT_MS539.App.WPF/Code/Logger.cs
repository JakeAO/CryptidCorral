using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace UAT_MS539.Code
{
    public class Logger : IDisposable
    {
        private const string LOG_PATH = "Logs/";

        public enum LogLevel
        {
            Debug = 0,
            Warning = 1,
            Error = 2,
            Exception = 3,
        }

        private StreamWriter _logStream = null;

        public Logger()
        {
            if (Application.Current?.Dispatcher != null)
                Application.Current.Dispatcher.UnhandledException += HandleGlobalException;

            string logPath = LOG_PATH + DateTime.Now.ToString("yy_MM_dd_hh_mm_ss") + ".log";
            FileInfo fi = new FileInfo(Path.GetFullPath(logPath));

            Directory.CreateDirectory(fi.DirectoryName);
            _logStream = File.CreateText(fi.FullName);
            _logStream.AutoFlush = true;
        }

        ~Logger()
        {
            Cleanup();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Cleanup();
        }

        private void Cleanup()
        {
            _logStream?.Close();
            _logStream?.Dispose();
            _logStream = null;
            if (Application.Current?.Dispatcher != null)
                Application.Current.Dispatcher.UnhandledException -= HandleGlobalException;
        }

        private void HandleGlobalException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log(e.Exception);
            _logStream?.Flush();
        }

        public void Log(Exception ex)
        {
            Log(LogLevel.Exception, $"{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        public void Log(LogLevel level, string message)
        {
            _logStream?.WriteLine($"[{DateTime.Now:hhmmss}][{level}] {message}");
        }
    }
}