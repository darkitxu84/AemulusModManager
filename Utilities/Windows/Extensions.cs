using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities.Windows
{
    public readonly struct Extension
    {
        public Extension(string filter, string title)
        {
            Filter = filter;
            Title = title;
        }

        public string Filter { get; }
        public string Title { get; }
    }

    // I don't think this is really necessary but it's cool
    public static class Extensions
    {
        public static readonly Extension PpssppCheat = new Extension("*.ini", "PPSSPP Cheat File");
        public static readonly Extension Ps2Iso = new Extension("*.iso", "PS2 ISO Image");
        public static readonly Extension PspIso = new Extension("*.iso", "PSP ISO Image");
        public static readonly Extension Exe = new Extension("*.exe", "Application");
        public static readonly Extension Elf = new Extension("*.elf", "PS2 Executable");
        public static readonly Extension Cpk = new Extension("*.cpk", "CPK File");
        public static readonly Extension Ps3Eboot = new Extension("*.bin","PS3 Executable (EBOOT.BIN)");
        public static readonly Extension SwitchRom = new Extension("*.xci;*.nsp", "Switch ROM");
        public static readonly Extension N3dsRom = new Extension("*.3ds;*.app;*.cxi;*.cci", "Nintendo 3DS ROM");
    }
}
