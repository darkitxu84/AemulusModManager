using System;
using System.Diagnostics;
using System.IO;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class ZipUtils
    {
        private const string Name = "7zip";
        private readonly static string Dir = $@"{Folders.Dependencies}\7z";

        // use 7Zip to extract 
        public static bool Extract(string filePath, string outputPath, string filter = "")
        {
            if (!ToolManager.IsValidInput(filePath, Name, "unzip"))
                return false;

            string _7zExe = @$"{Dir}\7z.exe";

            string args = $"x -y -bsp1 \"{filePath}\" -o\"{outputPath}\" {filter}";
            return ToolManager.Execute(Name, _7zExe, args, handleInput: true);
        }
    }
}
