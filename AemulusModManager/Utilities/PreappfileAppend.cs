using AemulusModManager.Utilities;
using AemulusModManager.Utilities.ToolsManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Path = System.IO.Path;

namespace AemulusModManager
{
    public static class PreappfileAppend
    {
        public static string GetChecksumString(string filePath)
        {
            string checksumString = null;

            // get md5 checksum of file
            using (var md5 = MD5.Create())
            {
                using var stream = File.OpenRead(filePath);
                // get hash
                byte[] currentFileSum = md5.ComputeHash(stream);
                // convert hash to string
                checksumString = BitConverter.ToString(currentFileSum).Replace("-", "");
            }

            return checksumString;
        }

        private static bool ValidatePath(string path, string folderName, string pacPath)
        {
            foreach (var file in Directory.GetFiles($@"{path}\mods\preappfile\{folderName}", "*", SearchOption.AllDirectories))
            {
                var folders = new List<string>(file.Split(char.Parse("\\")));
                int idx = folders.IndexOf(folderName);
                if (File.Exists($@"{pacPath}\{string.Join("\\", folders.Skip(idx + 1).ToArray())}"))
                    Utilities.ParallelLogger.Log($"[INFO] Validated that {file} was appended");
                else
                {
                    Utilities.ParallelLogger.Log($"[WARNING] {file} not appended");
                    return false;
                }
            }

            return true;
        }

        public static void Validate(string path, string cpkLang)
        {
            var validated = true;
            string data07Path = $@"{path}\data00007";
            string movie03Path = $@"{path}\movie00003";

            if (File.Exists($"{data07Path}.pac"))
            {
                Preappfile.Unpack($"{data07Path}.pac");
                validated = ValidatePath(path, Path.GetFileNameWithoutExtension(cpkLang), data07Path);
                PathUtils.DeleteIfExists(data07Path);
            }
            if (File.Exists($"{movie03Path}.pac"))
            {
                Preappfile.Unpack(movie03Path);
                validated = ValidatePath(path, "movie", movie03Path);
                PathUtils.DeleteIfExists(movie03Path);
            }
            // this is potentially dangerous, we can enter an infinite loop if something is really broken
            if (!validated)
            {
                Utilities.ParallelLogger.Log($"[WARNING] Not all appended files were validated, trying again");
                Append(path, cpkLang);
                Validate(path, cpkLang);
            }
        }

        public static void Append(string path, string cpkLang)
        {
            string originalP4GFolder = $@"{Folders.Original}\{Games.P4G}";

            // Check if required files are there
            if (!File.Exists($@"{path}\{cpkLang}"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find {path}\{cpkLang} for appending.");
                return;
            }

            // Delete modified pacs
            PathUtils.DeleteIfExists($@"{path}\data00007.pac");
            PathUtils.DeleteIfExists($@"{path}\movie00003.pac");

            var cpks = new List<(string Filename, int AppendIndex)>
            {
                (cpkLang, 7),
                ("movie.cpk", 3)
            };

            foreach (var (Filename, AppendIndex) in cpks)
            {
                // Backup cpk if not backed up already
                if (!File.Exists($@"{originalP4GFolder}\{Filename}"))
                {
                    Utilities.ParallelLogger.Log($@"[INFO] Backing up {Filename}");
                    File.Copy($@"{path}\{Filename}", $@"{originalP4GFolder}\{Filename}");
                }
                // Copy original cpk back if different
                if (GetChecksumString($@"{originalP4GFolder}\{Filename}") != GetChecksumString($@"{path}\{Filename}"))
                {
                    Utilities.ParallelLogger.Log($@"[INFO] Reverting {Filename} back to original");
                    File.Copy($@"{originalP4GFolder}\{Filename}", $@"{path}\{Filename}", true);
                }

                string input = $@"{path}\mods\preappfile\{Path.GetFileNameWithoutExtension(Filename)}";
                string output = $@"{path}\{Filename}";
                var appendInfo = new AppendInfo(AppendIndex, output);
                if (Directory.Exists(input))
                {
                    Utilities.ParallelLogger.Log($@"[INFO] Appending to {Filename}");
                    Preappfile.Append(input, appendInfo, output);
                }
            }
        }
    }
}
