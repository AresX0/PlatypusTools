using System;
using System.Collections.Generic;
using System.IO;

namespace PlatypusTools.Core.Services
{
    public static class FileCleaner
    {
        // TODO: Port logic from Get-FilteredFiles, Get-ProposedName, Rename-ItemCaseAware, etc.

        public static IEnumerable<FileInfo> GetFilteredFiles(string path, bool recurse, IEnumerable<string>? extensions = null)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) yield break;
            var search = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var fi in new DirectoryInfo(path).EnumerateFiles("*", search))
            {
                if (extensions == null)
                {
                    yield return fi;
                }
                else
                {
                    var ext = fi.Extension.ToLowerInvariant();
                    foreach (var e in extensions)
                    {
                        if (e.Equals(ext, StringComparison.OrdinalIgnoreCase)) { yield return fi; break; }
                    }
                }
            }
        }

        public static IEnumerable<string> GetFiles(string path, IEnumerable<string>? includePatterns = null, bool recurse = true)
        {
            var opts = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (!Directory.Exists(path)) return Array.Empty<string>();
            if (includePatterns == null) return Directory.EnumerateFiles(path, "*", opts);

            var files = new List<string>();
            foreach (var pat in includePatterns)
            {
                try { files.AddRange(Directory.EnumerateFiles(path, pat, opts)); } catch { }
            }
            return files.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        public static IList<string> RemoveFiles(IEnumerable<string> files, bool dryRun = true, string? backupPath = null)
        {
            var removed = new List<string>();
            foreach (var f in files)
            {
                try
                {
                    if (!File.Exists(f)) continue;
                    if (!dryRun)
                    {
                        if (!string.IsNullOrEmpty(backupPath))
                        {
                            try { Directory.CreateDirectory(backupPath); File.Copy(f, Path.Combine(backupPath, Path.GetFileName(f)), true); } catch { }
                        }
                        File.Delete(f);
                    }
                    removed.Add(f);
                }
                catch { }
            }
            return removed;
        }
    }
}