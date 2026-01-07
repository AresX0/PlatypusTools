using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace PlatypusTools.Core.Services
{
    public static class MetadataService
    {
        public static IDictionary<string, object?> GetMetadata(string filePath, string? exiftoolPath = null)
        {
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(filePath)) return result;

            var exif = MediaService.ResolveToolPath("exiftool", exiftoolPath);
            if (!string.IsNullOrEmpty(exif))
            {
                try
                {
                    using var proc = MediaService.StartTool(exif, $"-j \"{filePath}\"");
                    if (proc == null) return result;
                    var outJson = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(3000);
                    if (!string.IsNullOrWhiteSpace(outJson))
                    {
                        var arr = JsonSerializer.Deserialize<List<JsonElement>>(outJson);
                        if (arr != null && arr.Count > 0)
                        {
                            foreach (var prop in arr[0].EnumerateObject()) result[prop.Name] = prop.Value.ToString();
                        }
                    }
                }
                catch { }
                return result;
            }

            // Fallback: basic file info
            try
            {
                var fi = new FileInfo(filePath);
                result["Name"] = fi.Name;
                result["Length"] = fi.Length;
                result["Created"] = fi.CreationTimeUtc;
                result["Modified"] = fi.LastWriteTimeUtc;
            }
            catch { }
            return result;
        }
    }
}