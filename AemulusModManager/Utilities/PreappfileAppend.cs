using AemulusModManager.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Path = System.IO.Path;

namespace AemulusModManager
{
    public readonly struct AppendInfo
    {
        public AppendInfo(int index, string filePath)
        {
            Index = index;
            FilePath = filePath;
        }
        public int Index { get; }
        public string FilePath { get; }
    }

    public static class PreappfileAppend
    {
        public static void RunCommand(string inputPath, string outputPath = ".", string glob = null)
        {
            string preappfileAppendDir = $@"{Folders.Dependencies}\Preappfile\preappfile.exe";

            if (!File.Exists(preappfileAppendDir))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find Dependencies\preappfile\preappfile.exe. Please check if it was blocked by your anti-virus.");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = preappfileAppendDir,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = $"-i \"{inputPath}\" -o \"{outputPath}\" " + (glob != null ? $"--unpack-filter {glob}" : "")
            };

            using Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static void RunCommand(string inputPath, AppendInfo appendInfo, string outputPath = ".", string glob = null)
        {
            string preappfileAppendDir = $@"{Folders.Dependencies}\Preappfile\preappfile.exe";

            if (!File.Exists(preappfileAppendDir))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find Dependencies\preappfile\preappfile.exe. Please check if it was blocked by your anti-virus.");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = preappfileAppendDir,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = $"-i \"{inputPath}\" -o \"{outputPath}\" -a \"{appendInfo.FilePath}\" --pac-index {appendInfo.Index}" + (glob != null ? $"--unpack-filter {glob}" : "")
            };

            using Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

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
                RunCommand($"{data07Path}.pac");
                validated = ValidatePath(path, Path.GetFileNameWithoutExtension(cpkLang), data07Path);

                if (Directory.Exists(data07Path))
                    Directory.Delete(data07Path, true);
            }
            if (File.Exists($"{movie03Path}.pac"))
            {
                RunCommand(movie03Path);
                validated = ValidatePath(path, "movie", movie03Path);

                if (Directory.Exists($@"{path}\movie00003"))
                    Directory.Delete($@"{path}\movie00003", true);
            }
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

            // Backup cpk if not backed up already
            if (!File.Exists($@"{originalP4GFolder}\{cpkLang}"))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Backing up {cpkLang}.cpk");
                File.Copy($@"{path}\{cpkLang}", $@"Original\{Games.P4G}\{cpkLang}");
            }
            // Copy original cpk back if different
            if (GetChecksumString($@"{originalP4GFolder}\{cpkLang}") != GetChecksumString($@"{path}\{cpkLang}"))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Reverting {cpkLang} back to original");
                File.Copy($@"{originalP4GFolder}\{cpkLang}", $@"{path}\{cpkLang}", true);
            }
            if (!File.Exists($@"{originalP4GFolder}\movie.cpk"))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Backing up movie.cpk");
                File.Copy($@"{path}\movie.cpk", $@"{originalP4GFolder}\movie.cpk");
            }
            // Copy original cpk back if different
            if (GetChecksumString($@"{originalP4GFolder}\movie.cpk") != GetChecksumString($@"{path}\movie.cpk"))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Reverting movie.cpk back to original");
                File.Copy($@"{originalP4GFolder}\movie.cpk", $@"{path}\movie.cpk", true);
            }
            // Delete modified pacs
            if (File.Exists($@"{path}\data00007.pac"))
            {
                Utilities.ParallelLogger.Log($"[INFO] Deleting data00007.pac");
                File.Delete($@"{path}\data00007.pac");
            }
            if (File.Exists($@"{path}\movie00003.pac"))
            {
                Utilities.ParallelLogger.Log($"[INFO] Deleting movie00003.pac");
                File.Delete($@"{path}\movie00003.pac");
            }

            string inputPath = $@"{path}\mods\preappfile\{Path.GetFileNameWithoutExtension(cpkLang)}";
            string outputPath = $@"{path}\{cpkLang}";
            var appendInfo = new AppendInfo(7, outputPath);
            if (Directory.Exists(inputPath))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Appending to {cpkLang}");
                RunCommand(inputPath, appendInfo, outputPath);
            }

            inputPath = $@"{path}\mods\preappfile\movie";
            outputPath = $@"{path}\movie.cpk";
            appendInfo = new AppendInfo(3, outputPath);
            if (Directory.Exists(inputPath))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Appending to movie");
                RunCommand(inputPath, appendInfo, outputPath);
            }
        }
    }
}
