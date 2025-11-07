using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class AwbTools
    {
        private const string Name = "AwbTools";
        private readonly static string Dir = $@"{Folders.Dependencies}\AwbTools";

        public static bool Unpack(string input, string extension)
        {
            if (!ToolManager.IsValidInput(input, Name, "unpack"))
                return false;

            string unpackExe = $@"{Dir}\AWB_unpacker.exe";
            bool sucess = ToolManager.Execute(Name, unpackExe, $"\"{input}\"");
            
            if (sucess)
            {
                string awbPath = $@"{Path.GetDirectoryName(input)}\{Path.GetFileNameWithoutExtension(input)}";
                Directory.CreateDirectory(awbPath);

                List<string> files = new List<string>(Directory.EnumerateFiles($@"{input}_extracted_files"));
                foreach (var file in files)
                    File.Move(file, $@"{awbPath}\{Convert.ToString(int.Parse(Path.GetFileNameWithoutExtension(file), NumberStyles.HexNumber)).PadLeft(5, '0')}_streaming{extension}");
                Directory.Delete($@"{input}_extracted_files", true);
            }

            return sucess;
        }

        public static bool Repack(string input, bool automaticMove = false)
        {
            if (!ToolManager.IsValidInput(input, Name, "repack"))
                return false;

            string repackExe = $@"{Dir}\AWB_repacker.exe";
            bool sucess = ToolManager.Execute(Name, repackExe, $"\"{input}\"");

            if (sucess && automaticMove)
                File.Move($@"{Folders.Root}\OUT.AWB", Path.ChangeExtension(input, ".awb"), true);

            return sucess;
        }
    }
}
