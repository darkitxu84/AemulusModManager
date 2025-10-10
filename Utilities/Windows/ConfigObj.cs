using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using AemulusModManager.Utilities.Windows;
using AemulusModManager.Windows;

namespace AemulusModManager
{
    // Old config
    public class Config
    {
        // Keep to transfer data to new config
        public ObservableCollection<Package> package { get; set; }
        public string modDir { get; set; }
        public string exePath { get; set; }
        public string reloadedPath { get; set; }
        public bool emptySND { get; set; }
        public bool useCpk { get; set; }
        public string cpkLang { get; set; }
    }

    public sealed class AemulusConfig
    {

        private static AemulusConfig _instance = null;
        private AemulusConfig() { }
        private XmlSerializer xs;

        public string game { get; set; }
        public bool bottomUpPriority { get; set; }
        public bool updateAemulus { get; set; } = true;
        public bool darkMode { get; set; } = true;
        public ConfigP3F p3fConfig { get; set; }
        public ConfigP3P p3pConfig { get; set; }
        public ConfigP4G p4gConfig { get; set; }
        public ConfigP4GVita p4gVitaConfig { get; set; }
        public ConfigP5 p5Config { get; set; }
        public ConfigP5R p5rConfig { get; set; }
        public ConfigP5RSwitch p5rSwitchConfig { get; set; }
        public ConfigP5S p5sConfig { get; set; }
        public ConfigPQ pqConfig { get; set; }
        public ConfigPQ2 pq2Config { get; set; }
        public ConfigP1PSP p1pspConfig { get; set; }
        public double? LeftGridWidth { get; set; }
        public double? RightGridWidth { get; set; }
        public double? TopGridHeight { get; set; }
        public double? BottomGridHeight { get; set; }
        public double? RightTopGridHeight { get; set; }
        public double? RightBottomGridHeight { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public bool Maximized { get; set; }

        public static AemulusConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AemulusConfig();
                }
                return _instance;
            }
        }

        public void InitConfig()
        {
            _instance.p3fConfig = new ConfigP3F();
            _instance.p3pConfig = new ConfigP3P();
            _instance.p4gConfig = new ConfigP4G();
            _instance.p4gVitaConfig = new ConfigP4GVita();
            _instance.p5Config = new ConfigP5();
            _instance.p5rConfig = new ConfigP5R();
            _instance.p5rSwitchConfig = new ConfigP5RSwitch();
            _instance.p5sConfig = new ConfigP5S();
            _instance.pqConfig = new ConfigPQ();
            _instance.pq2Config = new ConfigPQ2();
            _instance.p1pspConfig = new ConfigP1PSP();

            xs = new XmlSerializer(typeof(AemulusConfig));
        }
        public void LoadConfig()
        {

        }

        public void UpdateConfig()
        {
            using (FileStream streamWriter = File.Create($@"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\Config\Config.xml"))
            {
                try
                {
                    xs.Serialize(streamWriter, _instance);
                }
                catch (Exception ex)
                {
                    Utilities.ParallelLogger.Log($@"[ERROR] Couldn't update Config\Config.xml ({ex.Message})");
                }
            }
        }
    }
    // there are a lot of attributes that are common in many configs
    public class DefaultConfig
    {
        public string modDir { get; set; }
        public bool deleteOldVersions { get; set; }
        public bool buildWarning { get; set; } = true;
        public bool buildFinished { get; set; } = true;
        public bool updateConfirm { get; set; } = true;
        public bool updateChangelog { get; set; } = true;
        public bool updateAll { get; set; } = true;
        public bool updatesEnabled { get; set; } = true;
        public string loadout { get; set; }
        public string lastUnpacked { get; set; }

    }

    public class ConfigP4G : DefaultConfig
    {
        public string exePath { get; set; }
        public string reloadedPath { get; set; }
        public bool emptySND { get; set; }
        public bool useCpk { get; set; }
        public string cpkLang { get; set; }
    }

    public class ConfigP4GVita : DefaultConfig
    {
        public string cpkName { get; set; } = "m0.cpk";
    }

    public class ConfigP1PSP : DefaultConfig
    {
        public string texturesPath { get; set; }
        public string cheatsPath { get; set; }
        public string isoPath { get; set; }
        public string launcherPath { get; set; }
        public bool createIso { get; set; } = false;
    }

    public class ConfigP3F : DefaultConfig
    {
        public string isoPath { get; set; }
        public string elfPath { get; set; }
        public string launcherPath { get; set; }
        public string cheatsPath { get; set; }
        public string cheatsWSPath { get; set; }
        public string texturesPath { get; set; }
        public bool advancedLaunchOptions { get; set; }
        public bool usePnachNewFormat { get; set; } = false;

    }
    public class ConfigP3P : DefaultConfig
    {
        public string texturesPath { get; set; }
        public string cheatsPath { get; set; }
        public string isoPath { get; set; }
        public string cpkName { get; set; } = "mod.cpk";
        public string launcherPath { get; set; }
    }

    public class ConfigP5 : DefaultConfig
    {
        public string gamePath { get; set; }
        public string launcherPath { get; set; }
        public string cpkName { get; set; } = "mod";
    }
    public class ConfigP5R : DefaultConfig
    {
        public string cpkName { get; set; } = "mod.cpk";
        public string language { get; set; } = "English";
        public string version { get; set; } = "1.02";
    }
    public class ConfigP5RSwitch : DefaultConfig
    {
        public string gamePath { get; set; }
        public string launcherPath { get; set; }
        public string language { get; set; } = "English";
    }
    public class ConfigPQ2 : DefaultConfig
    {
        public string ROMPath { get; set; }
        public string launcherPath { get; set; }
    }
    public class ConfigPQ : DefaultConfig
    {
        public string ROMPath { get; set; }
        public string launcherPath { get; set; }
    }

    public class ConfigP5S : DefaultConfig
    {

    }
    public class Packages
    {
        public ObservableCollection<Package> packages { get; set; }
        public bool showHiddenPackages { get; set; } = true;
    }
    public class Package
    {
        public string name { get; set; }
        public string path { get; set; }
        public bool enabled { get; set; }
        public string id { get; set; }
        public bool hidden { get; set; } = false;
        public string link { get; set; }
    }

    public class Metadata
    {
        public string name { get; set; }
        public string id { get; set; }
        public string author { get; set; }
        public string version { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public string skippedVersion { get; set; }
    }

    [Serializable, XmlRoot("Mod")]
    public class ModXmlMetadata
    {
        public string Id { get; set; }
        public string Game { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string UpdateUrl { get; set; }
    }

    public class DisplayedMetadata
    {
        public string name { get; set; }
        public string id { get; set; }
        public string author { get; set; }
        public bool enabled { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string path { get; set; }
        public string skippedVersion { get; set; }
        public bool hidden { get; set; } = false;
    }
}
