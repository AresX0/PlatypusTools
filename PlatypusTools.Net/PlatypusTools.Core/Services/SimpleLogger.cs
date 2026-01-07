using System;
using System.IO;

namespace PlatypusTools.Core.Services
{
    public static class SimpleLogger
    {
        private static readonly object _sync = new object();
        public static string? LogFile { get; set; }

        public static void Write(string message)
        {
            try
            {
                var entry = $"[{DateTime.UtcNow:O}] {message}\r\n";
                if (string.IsNullOrEmpty(LogFile)) Console.Write(entry);
                else
                {
                    lock (_sync)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LogFile) ?? ".");
                        File.AppendAllText(LogFile, entry);
                    }
                }
            }
            catch { /* best-effort logging */ }
        }
    }
}