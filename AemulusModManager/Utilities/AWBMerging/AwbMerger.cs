using AemulusModManager.Utilities.ToolsManager;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AemulusModManager.Utilities.AwbMerging
{
    internal class AwbMerger
    {
        private static bool AcbExists(string path) =>
            File.Exists(Path.ChangeExtension(path, ".acb"));
        
        private static bool AwbExists(string path) =>
            File.Exists(Path.ChangeExtension(path, ".awb"));

        public static bool SoundArchiveExists(string path) =>
            AcbExists(path) || AwbExists(path);
        
        private static void CopyAndUnpackArchive(string acbPath, string ogAcbPath, string extension)
        {
            ogAcbPath = Path.ChangeExtension(ogAcbPath, ".acb");
            acbPath = Path.ChangeExtension(acbPath, ".acb");
            string ogAwbPath = Path.ChangeExtension(ogAcbPath, ".awb");
            string awbPath = Path.ChangeExtension(acbPath, ".awb");
            Directory.CreateDirectory(Path.GetDirectoryName(acbPath));

            if (AwbExists(ogAwbPath))
            {
                ParallelLogger.Log($"[INFO] Copying over {ogAwbPath} to use as base.");
                File.Copy(ogAwbPath, awbPath, true);
            }
            else if (AwbExists(ogAwbPath = $@"{Path.GetDirectoryName(ogAwbPath)}\{Path.GetFileNameWithoutExtension(ogAwbPath)}_streamfiles.awb"))
            {
                ParallelLogger.Log($"[INFO] Copying over {ogAwbPath} to use as base.");
                awbPath = $@"{Path.GetDirectoryName(acbPath)}\{Path.GetFileName(ogAwbPath)}";
                File.Copy(ogAwbPath, awbPath, true);
            }

            if (AcbExists(ogAcbPath))
            {
                Utilities.ParallelLogger.Log($"[INFO] Copying over {ogAcbPath} to use as base.");
                File.Copy(ogAcbPath, acbPath, true);
                Utilities.ParallelLogger.Log($"[INFO] Unpacking {acbPath}");
                SonicAudioTools.AcbEditor(acbPath);
            }
            else
            {
                Utilities.ParallelLogger.Log($"[INFO] Unpacking {awbPath}");
                AwbTools.Unpack(awbPath, extension);
            }
        }

        public static void Merge(List<string> ModList, string game, string modDir)
        {
            List<string> acbs = new List<string>();
            foreach (string mod in ModList)
            {
                List<string> directories = new List<string>(Directory.EnumerateDirectories(mod, "*", SearchOption.AllDirectories));
                string[] AemIgnore = File.Exists($@"{mod}\Ignore.aem") ? File.ReadAllLines($@"{mod}\Ignore.aem") : null;

                foreach (string dir in directories)
                {
                    List<string> folders = new List<string>(dir.Split(char.Parse("\\")));
                    string ogAcbPath = $@"{Folders.Original}\{game}\{string.Join("\\", folders.ToArray())}";
                    if (!SoundArchiveExists(ogAcbPath))
                        continue;

                    string acbPath = $@"{modDir}\{string.Join("\\", folders.ToArray())}";
                    int idx = folders.IndexOf(Path.GetFileName(mod));
                    folders = folders.Skip(idx + 1).ToList();

                    List<string> files = new List<string>(Directory.GetFiles(dir));
                    foreach (string file in files)
                    {
                        if (AemIgnore != null && AemIgnore.Any(file.Contains))
                            continue;

                        string fileWithoutExt = Path.GetFileNameWithoutExtension(file);
                        string fileExt = Path.GetExtension(file);

                        if (!Directory.Exists(acbPath))
                        {
                            CopyAndUnpackArchive(acbPath, ogAcbPath, fileExt);
                            acbs.Add(acbPath);

                        }
                        string fileName = ( !fileWithoutExt.Contains('_') ) 
                            ? $@"{fileWithoutExt.PadLeft(5, '0')}{fileExt}" 
                            : $@"{Path.GetFileName(file)[..Path.GetFileName(file).IndexOf('_')].PadLeft(5, '0')}_streaming{fileExt}";
                        File.Copy(file, $@"{acbPath}\{fileName}", true);
                        Utilities.ParallelLogger.Log($"[INFO] Copying over {file} to {acbPath}");
                    }
                }
            }
            foreach (string acb in acbs)
            {
                Utilities.ParallelLogger.Log($"[INFO] Repacking {acb}");
                if (AcbExists(acb))
                    SonicAudioTools.AcbEditor(acb);
                else
                    AwbTools.Repack(acb, automaticMove: true);
                Directory.Delete(acb, true);
            }
        }
    }
}
