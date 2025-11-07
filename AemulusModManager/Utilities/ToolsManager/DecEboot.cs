using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class DecEboot
    {
        private const string Name = "DecEboot";
        private readonly static string Dir = $@"{Folders.Dependencies}\DecEboot";

        /// <summary>
        /// Decrypt an PSP EBOOT.BIN file with deceboot.exe
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static bool Decrypt(string input, string output)
        {
            if (!ToolManager.IsValidInput(input, Name, "decrypt"))
                return false;
            
            string decEbootExe = $@"{Dir}\deceboot.exe";

            Utilities.ParallelLogger.Log($"[INFO] Decrypting EBOOT.BIN in {input}...");
            return ToolManager.Execute(Name, decEbootExe, $"\"{input}\" \"{output}\"");
        }
    }
}
