using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class DecEboot
    {
        public static void Decrypt(string input, string output)
        {
            string decEbootDir = $@"{Folders.Dependencies}\DecEboot\deceboot.exe";

            ProcessStartInfo ebootDecoder = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = decEbootDir,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"\"{input}\" \"{output}}\""
            };

            Utilities.ParallelLogger.Log($"[INFO] Decrypting EBOOT.BIN");
            using (Process process = new Process())
            {
                process.StartInfo = ebootDecoder;
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
