using System.IO;
using System.Reflection;

namespace AemulusModManager.Utilities
{
    public static class Folders
    {
        public static readonly string Root;
        public static readonly string Original;
        public static readonly string Packages;
        public static readonly string Libraries;
        public static readonly string Dependencies;
        public static readonly string Config;
        public static readonly string Downloads;
        public static readonly string FilteredCpkCsv;

        static Folders()
        {
            Root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Original = $@"{Root}\Original";
            Packages = $@"{Root}\Packages";
            Libraries = $@"{Root}\Libraries";
            Dependencies = $@"{Root}\Dependencies";
            Config = $@"{Root}\Config";
            Downloads = $@"{Root}\Downloads";
            FilteredCpkCsv = $@"{Dependencies}\FilteredCpkCsv";
        }
    }
}
