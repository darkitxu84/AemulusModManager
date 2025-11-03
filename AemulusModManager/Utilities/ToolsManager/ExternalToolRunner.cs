using AtlusScriptLibrary.FlowScriptLanguage.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.ToolsManager
{
    // there're generic validations and logic that can be extracted here
    public static class ExternalToolRunner
    {
        public static bool Execute(string toolName, string executablePath, string args, bool handleInput = false)
        {
            if (!File.Exists(executablePath))
            {
                ParallelLogger.Log($"[ERROR] Couldn't find {toolName} at {executablePath}. Check antivirus exclusions.");
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = executablePath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args,
                RedirectStandardOutput = handleInput
            };

            using Process process = new Process() { StartInfo = startInfo };
            process.Start();

            if (handleInput)
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    ParallelLogger.Log($"[INFO] {toolName}: {line}");
                }
            }
            process.WaitForExit();

            return true;
        }
    }
}
