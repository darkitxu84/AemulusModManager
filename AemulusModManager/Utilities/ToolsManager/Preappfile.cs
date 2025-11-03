using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class Preappfile
    {
        const string ToolName = "Preappfile";
        public static bool Unpack(string input, string output = ".", string unpackFilter = null)
        {
            if (!File.Exists(input))
            {
                ParallelLogger.Log($"[ERROR] {ToolName}: Error trying to unpack. Couldn't find {input}.");
                return false;
            }

            string preappfileDir = $@"{Folders.Dependencies}\Preappfile\preappfile.exe";

            string args = $"-i \"{input}\" -o \"{output}\"";
            if (unpackFilter != null)
               args += $" --unpack-filter {unpackFilter}";
            
            Utilities.ParallelLogger.Log($"[INFO] Unpacking {input} with preappfile");
            return ExternalToolRunner.Execute(ToolName, preappfileDir, args, handleInput: true);
        }
    }
}
