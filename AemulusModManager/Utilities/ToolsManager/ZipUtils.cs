using System;
using System.Diagnostics;
using System.IO;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class ZipUtils
    {
        // use 7Zip to extract 
        public static bool Extract(string filePath, string outputPath, string filter = "")
        {
            string _7zDir = @$"{Folders.Dependencies}\7z\7z.exe";

            if (!File.Exists(filePath))
            {
                ParallelLogger.Log($"[ERROR] Couldn't find {filePath}.");
                return false;
            }
            if (!File.Exists(_7zDir))
            {
                ParallelLogger.Log($"[ERROR] Couldn't find 7-Zip at {_7zDir}. Please check if it was blocked by your anti-virus");
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = _7zDir,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = $"x -y -bsp1 \"{filePath}\" -o\"{outputPath}\" {filter}"
            };

            using Process process = new Process() { StartInfo = startInfo };
            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                ParallelLogger.Log($"[INFO] 7zip: {line}");
            }
            process.WaitForExit();

            return true;
        }
    }
}
