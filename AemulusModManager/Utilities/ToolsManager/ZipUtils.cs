using System;
using System.Diagnostics;
using System.IO;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class ZipUtils
    {
        const string ToolName = "7zip";

        // use 7Zip to extract 
        public static bool Extract(string filePath, string outputPath, string filter = "")
        {
            string _7zDir = @$"{Folders.Dependencies}\7z\7z.exe";

            if (!File.Exists(filePath))
            {
                ParallelLogger.Log($"[ERROR] {ToolName}: Error trying to unzip. Couldn't find {filePath}.");
                return false;
            }

            string args = $"x -y -bsp1 \"{filePath}\" -o\"{outputPath}\" {filter}";
            return ExternalToolRunner.Execute(ToolName, _7zDir, args, handleInput: true);
        }
    }
}
