using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class DecEboot
    {
        const string ToolName = "DecEboot";
        public static bool Decrypt(string input, string output)
        {
            if (!File.Exists(input))
            {
                ParallelLogger.Log($"[ERROR] {ToolName}: Error trying to decrypt. Couldn't find {input}.");
                return false;
            }

            string decEbootDir = $@"{Folders.Dependencies}\DecEboot\deceboot.exe";

            Utilities.ParallelLogger.Log($"[INFO] Decrypting EBOOT.BIN in {input}...");
            return ExternalToolRunner.Execute(ToolName, decEbootDir, $"\"{input}\" \"{output}\"");
        }
    }
}
