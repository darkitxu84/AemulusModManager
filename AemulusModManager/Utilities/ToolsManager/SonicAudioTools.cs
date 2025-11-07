using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.ToolsManager
{
    public static class SonicAudioTools
    {
        private const string Name = "SonicAudioTools";
        private static readonly string Dir = $@"{Folders.Dependencies}\SonicAudioTools";

        public static bool AcbEditor(string input)
        {
            if (!ToolManager.IsValidInput(input, Name, "process"))
                return false;

            string acbEditorExe = $@"{Dir}\AcbEditor.exe";
            return ToolManager.Execute(Name, acbEditorExe, $"\"{input}\"", handleInput: true);

        }
    }
}
