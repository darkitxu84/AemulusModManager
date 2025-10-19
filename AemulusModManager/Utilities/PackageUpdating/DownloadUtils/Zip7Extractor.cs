using Onova.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.PackageUpdating.DownloadUtils
{
    /// <summary>
    /// Extracts files from 7z-archived packages.
    /// </summary>
    public class Zip7Extractor : IPackageExtractor
    {
        public async Task ExtractPackageAsync(string sourceFilePath, string destDirPath,
            IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            Utilities.ParallelLogger.Log($"[INFO] Extracting {sourceFilePath}");
            bool success = ZipUtils.Extract(sourceFilePath, destDirPath);
            if (!success)
                return;

            // TODO Check if it actually succeeded (by reading the command output I guess)
            Utilities.ParallelLogger.Log($"[INFO] Done Extracting {sourceFilePath}");
            File.Delete(@$"{sourceFilePath}");
            Utilities.ParallelLogger.Log(@$"[INFO] Deleted {sourceFilePath}");
            // Move the folders to the right place
            string parentPath = Directory.GetParent(destDirPath).FullName;
            Directory.Move(Directory.GetDirectories(destDirPath)[0], $@"{parentPath}\Aemulus");
            Directory.Delete(destDirPath);
            Directory.Move($@"{parentPath}\Aemulus", destDirPath);
        }
    }
}
