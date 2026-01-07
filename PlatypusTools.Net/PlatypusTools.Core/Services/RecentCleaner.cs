using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlatypusTools.Core.Services
{
    public class RecentMatch
    {
        public string Type { get; set; } = "Shortcut";
        public string Path { get; set; } = string.Empty;
        public string? Target { get; set; }
        public IList<string>? MatchedPaths { get; set; }
    }

    public static class RecentCleaner
    {
        public static IList<RecentMatch> RemoveRecentShortcuts(IEnumerable<string> targetDirs, bool dryRun = true, bool includeSubDirs = true, string? backupPath = null, string? recentFolder = null)
        {
            var results = new List<RecentMatch>();
            if (targetDirs == null) { return results; }
            var recent = recentFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            if (!Directory.Exists(recent)) { return results; }

            var lnkFiles = Directory.EnumerateFiles(recent, "*.lnk", SearchOption.AllDirectories);
            foreach (var lnk in lnkFiles)
            {
                try
                {
                    var target = ResolveShortcutTarget(lnk);
                    if (string.IsNullOrEmpty(target)) { continue; }

                    string compTarget;
                    try { compTarget = Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
                    catch { compTarget = target; }

                    foreach (var dir in targetDirs)
                    {
                        if (string.IsNullOrWhiteSpace(dir)) { continue; }
                        string compDir;
                        try { compDir = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
                        catch { compDir = dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }

                        bool isMatch;
                        if (includeSubDirs)
                        {
                            isMatch = compTarget.StartsWith(compDir, StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            var parent = Path.GetDirectoryName(compTarget);
                            isMatch = string.Equals(parent, compDir, StringComparison.OrdinalIgnoreCase);
                        }

                        if (isMatch)
                        {
                            results.Add(new RecentMatch { Type = "Shortcut", Path = lnk, Target = target });
                            if (!dryRun)
                            {
                                if (!string.IsNullOrEmpty(backupPath))
                                {
                                    try { Directory.CreateDirectory(backupPath); File.Copy(lnk, Path.Combine(backupPath, Path.GetFileName(lnk)), true); } catch { }
                                }
                                try { File.Delete(lnk); } catch { }
                            }
                            break;
                        }
                    }
                }
                catch { /* keep rough-port tolerant */ }
            }
            // TODO: Jump Lists, pinned items, Explorer MRU, etc. — complex formats to port
            return results;
        }

        private static string? ResolveShortcutTarget(string shortcutPath)
        {
            try
            {
                // Use WSH Shell COM to resolve shortcut if available
                var wsh = Type.GetTypeFromProgID("WScript.Shell");
                if (wsh == null) { return null; }
                dynamic shell = Activator.CreateInstance(wsh);
                dynamic lnk = shell.CreateShortcut(shortcutPath);
                string target = (string)lnk.TargetPath;
                return target;
            }
            catch
            {
                return null;
            }
        }
    }
}