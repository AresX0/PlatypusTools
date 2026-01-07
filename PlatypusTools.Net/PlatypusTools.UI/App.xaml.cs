using PlatypusTools.Core.Services;
using System;
using System.IO;
using System.Windows;

namespace PlatypusTools.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load app config and configure logging
            var cfg = AppConfig.Load();

            // Determine log file: prefer configured, otherwise use %LOCALAPPDATA%\PlatypusTools\logs\platypustools.log
            if (!string.IsNullOrWhiteSpace(cfg.LogFile))
            {
                try
                {
                    var dir = Path.GetDirectoryName(cfg.LogFile) ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    Directory.CreateDirectory(dir);
                    SimpleLogger.LogFile = cfg.LogFile;
                }
                catch (Exception ex)
                {
                    // If we fail to use configured path, fallback to default
                    Console.WriteLine($"Failed to use configured log file '{cfg.LogFile}': {ex.Message}");
                }
            }

            if (string.IsNullOrWhiteSpace(SimpleLogger.LogFile))
            {
                var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var logDir = Path.Combine(local, "PlatypusTools", "logs");
                Directory.CreateDirectory(logDir);
                SimpleLogger.LogFile = Path.Combine(logDir, "platypustools.log");
            }

            // Set minimum log level from config if provided
            if (!string.IsNullOrWhiteSpace(cfg.LogLevel) && Enum.TryParse<LogLevel>(cfg.LogLevel, true, out var lvl))
            {
                SimpleLogger.MinLevel = lvl;
            }

            SimpleLogger.Info($"Application starting. LogFile='{SimpleLogger.LogFile}', MinLevel={SimpleLogger.MinLevel}");
        }
    }
}