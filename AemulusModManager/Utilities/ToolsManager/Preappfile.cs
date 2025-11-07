using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.ToolsManager
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

    public static class Preappfile
    {
        private const string Name = "Preappfile";
        private readonly static string Dir = $@"{Folders.Dependencies}\Preappfile";

        /// <summary>
        /// Unpack a preappfile PAC file with preappfile.exe
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="unpackFilter"></param>
        /// <param name="pacIndex"></param>
        /// <returns></returns>
        public static bool Unpack(string input, string output = ".", string unpackFilter = null, int pacIndex = 0)
        {
            if (!ToolManager.IsValidInput(input, Name, "unpack"))
                return false;

            string preappfileExe = $@"{Dir}\preappfile.exe";

            string args = $"-i \"{input}\" -o \"{output}\"";
            if (unpackFilter != null)
               args += $" --unpack-filter {unpackFilter}";
            
            Utilities.ParallelLogger.Log($"[INFO] Unpacking {input} with preappfile");
            return ToolManager.Execute(Name, preappfileExe, args, handleInput: true);
        }

        /// <summary>
        /// Append a file to a preappfile PAC file with preappfile.exe
        /// </summary>
        /// <param name="input"></param>
        /// <param name="appendInfo"></param>
        /// <param name="output"></param>
        /// <param name="glob"></param>
        /// <returns></returns>
        public static bool Append(string input, AppendInfo appendInfo, string output = ".", string glob = null)
        {
            if (!ToolManager.IsValidInput(input, Name, "append") || !ToolManager.IsValidInput(appendInfo.FilePath, Name, "append"))
                return false;

            string preappfileExe = $@"{Dir}\preappfile.exe";

            string args = $"-i \"{input}\" -o \"{output}\" -a \"{appendInfo.FilePath}\" --pac-index {appendInfo.Index}";
            if (glob != null)
                args += $" --unpack-filter {glob}";

            Utilities.ParallelLogger.Log($"[INFO] Appending {appendInfo.FilePath} to {input} with preappfile");
            return ToolManager.Execute(Name, preappfileExe, args, handleInput: true);
        }
    }
}
