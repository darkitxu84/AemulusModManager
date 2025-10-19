using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AemulusModManager.Utilities
{
    public static class Folders
    {
        public static readonly string Root;
        public static readonly string Original;
        public static readonly string Packages;
        public static readonly string Libraries;
        public static readonly string Dependencies;

        static Folders()
        {
            Root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Original = $@"{Root}\Original";
            Packages = $@"{Root}\Packages";
            Libraries = $@"{Root}\Libraries";
            Dependencies = $@"{Root}\Dependencies";
        }
    }
}
