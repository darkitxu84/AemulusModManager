using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AemulusModManager.Utilities.Windows
{
    public static class FilePicker
    {
        public static string SelectFile(string title, in Extension extension, string mustContain = null, string exactMatch = null)
        {
            var openFile = new CommonOpenFileDialog()
            {
                EnsurePathExists = true,
                EnsureValidNames = true,
                Title = title,
            };
            openFile.Filters.Add(new CommonFileDialogFilter(extension.Title, extension.Filter));

            // If the user does not select a file we return null
            if (openFile.ShowDialog() != CommonFileDialogResult.Ok)
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
            var openFile = new CommonOpenFileDialog()
            {
                EnsurePathExists = true,
                EnsureValidNames = true,
                Title = title,
            };
            openFile.Filters.Add(new CommonFileDialogFilter(extension.Title, extension.Filter));

            // If the user does not select a file we return null
            if (openFile.ShowDialog() != CommonFileDialogResult.Ok)
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
            var openFolder = new CommonOpenFileDialog
            {
                AllowNonFileSystemItems = true,
                IsFolderPicker = true,
                EnsurePathExists = true,
                EnsureValidNames = true,
                Multiselect = false,
                Title = title
            };

            if (openFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                ParallelLogger.Log("[WARNING] No folder specified.");
                return null;
            }

            return openFolder.FileName;
        }
    }
}
