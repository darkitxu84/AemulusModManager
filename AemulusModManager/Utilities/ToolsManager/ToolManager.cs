using System.Diagnostics;
using System.IO;

namespace AemulusModManager.Utilities.ToolsManager
{
    // there're generic validations and logic that can be extracted here
    public static class ToolManager
    {
        /// <summary>
        /// Executes an external app with given arguments
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="executablePath"></param>
        /// <param name="args"></param>
        /// <param name="handleInput"></param>
        /// <returns></returns>
        public static bool Execute(string toolName, string executablePath, string args, bool handleInput = false)
        {
            if (!File.Exists(executablePath))
            {
                ParallelLogger.Log($"[ERROR] {toolName}: Couldn't find {toolName} at {executablePath}. Check antivirus exclusions.");
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

        public static bool IsValidInput(string input, string toolName, string action)
        {
            if (!File.Exists(input) || !Directory.Exists(input))
            {
                ParallelLogger.Log($"[ERROR] {toolName}: Error trying to {action}. Couldn't find {input}.");
                return false;
            }

            return true;
        }
    }

}
