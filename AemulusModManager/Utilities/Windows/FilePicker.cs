using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace AemulusModManager.Utilities.Windows
{
    public static class FilePicker
    {
        public static string SelectFile(string title, in Extension extension, string mustContain = null, string exactMatch = null)
        {
            var openFile = new OpenFileDialog()
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = title,
                Filter = $"{extension.Title}|{extension.Filter}"
            };

            // If the user does not select a file we return null
            if (!(bool)openFile.ShowDialog())
            {
                Utilities.ParallelLogger.Log($"[WARNING] No {extension.Filter} file specified.");
                return null;
            }

            if (mustContain != null && !Path.GetFileName(openFile.FileName).Contains(mustContain, StringComparison.OrdinalIgnoreCase))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Invalid {extension.Filter}. The file must contain: {mustContain}");
                return null;
            }
            if (exactMatch != null && Path.GetFileName(openFile.FileName) != exactMatch)
            {
                Utilities.ParallelLogger.Log($"[ERROR] Invalid {extension.Filter}. The file must be exact match with: {exactMatch}");
                return null;
            }

            return openFile.FileName;
        }

        public static string SelectFile(string title, in Extension extension, string[] exactMatch)
        {
            var openFile = new OpenFileDialog()
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = title,
                Filter = $"{extension.Title}({extension.Filter})|{extension.Filter}"
            };

            // If the user does not select a file we return null
            if (!(bool)openFile.ShowDialog())
            {
                Utilities.ParallelLogger.Log($"[WARNING] No {extension.Filter} file specified.");
                return null;
            }

            if (!exactMatch.Contains(Path.GetFileName(openFile.FileName)))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Invalid {extension.Filter}. The file must be exact match with: {String.Join(", ", exactMatch)}");
                return null;
            }

            return openFile.FileName;
        }

        public static string SelectFolder(string title)
        {
            var openFolder = new OpenFolderDialog
            {
                ValidateNames = true,
                Multiselect = false,
                Title = title
            };

            if (!(bool)openFolder.ShowDialog())
            {
                ParallelLogger.Log("[WARNING] No folder specified.");
                return null;
            }

            return openFolder.FolderName;
        }
    }
}
