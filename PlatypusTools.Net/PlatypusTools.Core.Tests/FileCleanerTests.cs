using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlatypusTools.Core.Services;
using System.IO;
using System.Linq;

namespace PlatypusTools.Core.Tests
{
    [TestClass]
    public class FileCleanerTests
    {
        [TestMethod]
        public void GetFiles_ReturnsCreatedFile()
        {
            var tmp = Path.Combine(Path.GetTempPath(), "pt_filecleaner_test");
            if (Directory.Exists(tmp)) Directory.Delete(tmp, true);
            Directory.CreateDirectory(tmp);
            var f = Path.Combine(tmp, "a.txt");
            File.WriteAllText(f, "x");

            var files = FileCleaner.GetFiles(tmp, new[] { "*.txt" }).ToList();
            Assert.IsTrue(files.Any(s => s.EndsWith("a.txt")));
        }

        [TestMethod]
        public void RemoveFiles_DryRun_DoesNotDelete()
        {
            var tmp = Path.Combine(Path.GetTempPath(), "pt_filecleaner_test2");
            if (Directory.Exists(tmp)) Directory.Delete(tmp, true);
            Directory.CreateDirectory(tmp);
            var f = Path.Combine(tmp, "b.txt");
            File.WriteAllText(f, "x");

            var removed = FileCleaner.RemoveFiles(new[] { f }, dryRun: true);
            Assert.IsTrue(File.Exists(f));
            Assert.IsTrue(removed.Contains(f));
        }
    }
}