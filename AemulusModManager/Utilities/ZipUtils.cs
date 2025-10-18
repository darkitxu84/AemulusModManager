using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities
{
    public static class ZipUtils
    {
        // use 7Zip to extract 
        public static void Extract(string filePath, string outputPath, string filter = "")
        {
            var config = AemulusConfig.Instance;
            string _7zDir = @$"{config.aemPath}\Dependencies\7z\7z.exe";

            if (!File.Exists(filePath))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {filePath}.");
                return;
            }
            if (!File.Exists(_7zDir))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find 7-Zip at {_7zDir}. Please check if it was blocked by your anti-virus");
                return;
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
                if (String.IsNullOrWhiteSpace(line))
                    continue;
                ParallelLogger.Log($"[INFO] 7zip: {line}");
            }
            process.WaitForExit();
        }
    }
}
