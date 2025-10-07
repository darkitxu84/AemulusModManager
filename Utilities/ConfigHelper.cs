using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AemulusModManager
{
    public static class ConfigHelper
    {
        public static string SelectFile(string title, string extension, string mustContain = null)
        {
            var typesMapping = new Dictionary<string, string>
            {
                [".iso"] = "ISO Image",
                [".elf"] = "PS2 Executable",
                [".exe"] = "Application"
            };
            var extensionTitleMapping = new Dictionary<string, string>
            {
                [".iso"] = "ISO",
                [".elf"] = "ELF/SLUS",
                [".exe"] = "EXE"
            };

            string type = typesMapping[extension];
            var openFile = new CommonOpenFileDialog();
            openFile.Filters.Add(new CommonFileDialogFilter(type, $"*{extension}"));
            openFile.EnsurePathExists = true;
            openFile.EnsureValidNames = true;
            openFile.Title = title;
       
            // If the user does not select a file we return null
            if (openFile.ShowDialog() != CommonFileDialogResult.Ok)
            {
                Utilities.ParallelLogger.Log($"[WARNING] No {extensionTitleMapping[extension]} file specified.");
                return null;
            }
            if (mustContain != null)
            {
                if (!Path.GetFileName(openFile.FileName).ToLower().Contains(mustContain))
                {
                    Utilities.ParallelLogger.Log($"[ERROR] Invalid {extensionTitleMapping[extension]}");
                    return null;
                }

            }

            return openFile.FileName;
        }

        public static string SelectFolder(string title)
        {
            var openFolder = new CommonOpenFileDialog();
            openFolder.AllowNonFileSystemItems = true;
            openFolder.IsFolderPicker = true;
            openFolder.EnsurePathExists = true;
            openFolder.EnsureValidNames = true;
            openFolder.Multiselect = false;
            openFolder.Title = title;
            if (openFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                Utilities.ParallelLogger.Log("[WARNING] No folder specified.");
                return null;
            }

            return openFolder.FileName;
        }
    }
}
