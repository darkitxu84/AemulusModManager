﻿using AemulusModManager.Utilities;
using CriFsV2Lib;
using CriFsV2Lib.Definitions.Interfaces;
using CriFsV2Lib.Definitions.Structs;
using CriFsV2Lib.Definitions.Utilities;
using CriFsV2Lib.Encryption.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AemulusModManager
{
    public static class PacUnpacker
    {
        internal class FileToExtract : IBatchFileExtractorItem
        {
            public string FullPath { get; set; }
            public CpkFile File { get; set; }
            public FileToExtract(string _fullPath, CpkFile _file)
            {
                FullPath = _fullPath;
                File = _file;
            }
        }

        //P1PSP
        public static async Task UnzipAndUnBin(string iso)
        {
            if (!File.Exists(iso))
            {
                Console.Write($"[ERROR] Couldn't find {iso}. Please correct the file path in config.");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P1PSP}";
            string ebootPath = $@"{pathToExtract}\PSP_GAME\SYSDIR";

            ZipUtils.Extract(iso, pathToExtract);
            File.Move($@"{ebootPath}\EBOOT.BIN", $@"{ebootPath}\EBOOT_ENC.BIN");

            ProcessStartInfo ebootDecoder = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = $@"{Folders.Dependencies}\DecEboot\deceboot.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "\"" + $@"{ebootPath}\EBOOT_ENC.BIN" + "\" \"" + $@"{ebootPath}\EBOOT_ENC.BIN" + "\""
            };

            Utilities.ParallelLogger.Log($"[INFO] Decrypting EBOOT.BIN");
            using (Process process = new Process())
            {
                process.StartInfo = ebootDecoder;
                process.Start();
                process.WaitForExit();
            }
            File.Delete($@"{ebootPath}\EBOOT_ENC.BIN");

            Utilities.ParallelLogger.Log("[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P3F
        public static async Task Unzip(string iso)
        {
            if (!File.Exists(iso))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {iso}. Please correct the file path in config.");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P3F}";
            const string filesFilter = "*.BIN *.PAK *.PAC *.TBL *.SPR *.BF *.BMD *.PM1 *.bf *.bmd *.pm1 *.FPC -r";

            ZipUtils.Extract(iso, pathToExtract, filter: "BTL.CVM DATA.CVM");
            ZipUtils.Extract($@"{pathToExtract}\BTL.CVM", $@"{pathToExtract}\BTL", filesFilter);
            ZipUtils.Extract($@"{pathToExtract}\DATA.CVM", $@"{pathToExtract}\DATA", filesFilter);
            File.Delete($@"{pathToExtract}\BTL.CVM");
            File.Delete($@"{pathToExtract}\DATA.CVM");

            Utilities.ParallelLogger.Log($"[INFO] Extracting base files from DATA.CVM");
            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P3P
        public static async Task UnzipAndUnpackCPK(string iso)
        {
            if (!File.Exists(iso))
            {
                Console.Write($"[ERROR] Couldn't find {iso}. Please correct the file path in config.");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P3P}";
            const string umd0PathFilter = @"PSP_GAME\USRDIR\umd0.cpk";

            Utilities.ParallelLogger.Log($"[INFO] Extracting umd0.cpk from {iso}");
            ZipUtils.Extract(iso, pathToExtract, filter: umd0PathFilter);
            string[] umd0Files = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_umd0.csv");
            string umd0Path = $@"{pathToExtract}\{umd0PathFilter}";

            Utilities.ParallelLogger.Log($"[INFO] Extracting files from umd0.cpk");
            if (File.Exists(umd0Path))
                CriFsUnpack(umd0Path, pathToExtract, umd0Files);
            else
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find {pathToExtract}\{umd0PathFilter}.");

            Utilities.ParallelLogger.Log("[INFO] Unpacking extracted files");
            ExtractWantedFiles($@"{pathToExtract}\data");

            if (Directory.Exists($@"{pathToExtract}\PSP_GAME"))
                Directory.Delete($@"{pathToExtract}\PSP_GAME", true);

            Utilities.ParallelLogger.Log("[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P4G
        public static void Unpack(string directory, string cpk)
        {
            if (!Directory.Exists(directory))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {directory}. Please correct the file path in config.");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P4G}";
            Directory.CreateDirectory(pathToExtract);

            List<string> pacs = new List<string>();
            List<string> globs = new List<string> 
            { 
                "*[!0-9].bin", "*2[0-1][0-9].bin", "*.arc", "*.pac", "*.pack", "*.bf", "*.bmd", "*.pm1" 
            };

            switch (cpk)
            {
                case "data_e.cpk":
                    pacs.Add("data00004.pac");
                    pacs.Add("data_e.cpk");
                    break;
                case "data.cpk":
                    pacs.Add("data00000.pac");
                    pacs.Add("data00001.pac");
                    pacs.Add("data00003.pac");
                    pacs.Add("data.cpk");
                    break;
                case "data_k.cpk":
                    pacs.Add("data00005.pac");
                    pacs.Add("data_k.cpk");
                    break;
                case "data_c.cpk":
                    pacs.Add("data00006.pac");
                    pacs.Add("data_c.cpk");
                    break;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = $@"{Folders.Dependencies}\Preappfile\preappfile.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            if (!File.Exists(startInfo.FileName))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {startInfo.FileName}. Please check if it was blocked by your anti-virus.");
                return;
            }

            foreach (var pac in pacs)
            {
                Utilities.ParallelLogger.Log($"[INFO] Unpacking files for {pac}...");
                foreach (var glob in globs)
                {
                    startInfo.Arguments = $@"-i ""{directory}\{pac}"" -o ""{pathToExtract}\{Path.GetFileNameWithoutExtension(pac)}"" --unpack-filter {glob}";
                    using Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    while (!process.HasExited)
                    {
                        string text = process.StandardOutput.ReadLine();
                        if (!String.IsNullOrEmpty(text))
                            Utilities.ParallelLogger.Log($"[INFO] {text}");
                    }
                }
                ExtractWantedFiles($@"{pathToExtract}\{Path.GetFileNameWithoutExtension(pac)}");
            }
            if (File.Exists($@"{directory}\{cpk}") && !File.Exists($@"{pathToExtract}\{cpk}"))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Backing up {cpk}");
                File.Copy($@"{directory}\{cpk}", $@"{pathToExtract}\{cpk}", true);
            }
            if (File.Exists($@"{directory}\movie.cpk") && !File.Exists($@"{pathToExtract}\movie.cpk"))
            {
                Utilities.ParallelLogger.Log($@"[INFO] Backing up movie.cpk");
                File.Copy($@"{directory}\movie.cpk", $@"{pathToExtract}\movie.cpk", true);
            }

            Utilities.ParallelLogger.Log("[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P5 PS3
        public static async Task UnpackP5CPK(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {directory}. Please correct the file path in config.");
                return;
            }
            if (!File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_data.csv")
                || !File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_ps3.csv"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find CSV files used for unpacking in Dependencies\FilteredCpkCsv");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P5}";
            Directory.CreateDirectory(pathToExtract);

            if (File.Exists($@"{directory}\ps3.cpk.66600") 
                && File.Exists($@"{directory}\ps3.cpk.66601") 
                && File.Exists($@"{directory}\ps3.cpk.66602")
                && !File.Exists($@"{directory}\ps3.cpk"))
            {
                Console.Write("[INFO] Combining ps3.cpk parts");
                ProcessStartInfo cmdInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = @"CMD.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $@"/C copy /b ""{directory}\ps3.cpk.66600"" + ""{directory}\ps3.cpk.66601"" + ""{directory}\ps3.cpk.66602"" ""{directory}\ps3.cpk"""
                };

                using Process process = new Process();
                process.StartInfo = cmdInfo;
                process.Start();
                process.WaitForExit();
            }

            string[] dataFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_data.csv");
            string[] ps3Files = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_ps3.csv");

            Utilities.ParallelLogger.Log($"[INFO] Extracting data.cpk");
            if (File.Exists($@"{directory}\data.cpk"))
                CriFsUnpack($@"{directory}\data.cpk", pathToExtract, dataFiles);
            else
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find data.cpk in {directory}.");

            Utilities.ParallelLogger.Log($"[INFO] Extracting ps3.cpk");
            if (File.Exists($@"{directory}\ps3.cpk"))
                CriFsUnpack($@"{directory}\ps3.cpk", pathToExtract, ps3Files);
            else
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find ps3.cpk in {directory}.");

            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P5R PS4
        public static async Task UnpackP5RCPKs(string directory, string language, string version)
        {
            if (!Directory.Exists(directory))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {directory}. Please correct the file path.");
                return;
            }
            if (!File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_dataR.csv")
                || !File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_ps4R.csv"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find CSV files used for unpacking in Dependencies\FilteredCpkCsv");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P5R}";
            Directory.CreateDirectory(pathToExtract);

            string[] dataRFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_dataR.csv");
            string[] ps4RFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_ps4R.csv");

            Utilities.ParallelLogger.Log($"[INFO] Extracting dataR.cpk");
            if (File.Exists($@"{directory}\dataR.cpk"))
                CriFsUnpack($@"{directory}\dataR.cpk", pathToExtract, dataRFiles);
            else
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find dataR.cpk in {directory}.");

            Utilities.ParallelLogger.Log($"[INFO] Extracting ps4R.cpk");
            if (File.Exists($@"{directory}\ps4R.cpk"))
                CriFsUnpack($@"{directory}\ps4R.cpk", pathToExtract, ps4RFiles);
            else
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find ps4R.cpk in {directory}.");

            if (language != "English")
            {
                string[] dataRLocalizedFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_dataR_Localized.csv");
                var localizedCpk = String.Empty;
                switch (language)
                {
                    case "French":
                        localizedCpk = "dataR_F.cpk";
                        break;
                    case "Italian":
                        localizedCpk = "dataR_I.cpk";
                        break;
                    case "German":
                        localizedCpk = "dataR_G.cpk";
                        break;
                    case "Spanish":
                        localizedCpk = "dataR_S.cpk";
                        break;
                }
                Utilities.ParallelLogger.Log($"[INFO] Extracting {localizedCpk}");
                if (File.Exists($@"{directory}\{localizedCpk}"))
                    CriFsUnpack($@"{directory}\{localizedCpk}", pathToExtract, dataRLocalizedFiles);
                else
                    Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {localizedCpk} in {directory}.");
            }

            // Extract patch2R.cpk files
            if (version == ">= 1.02")
            {
                string[] patch2RFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_patch2R.csv");
                Utilities.ParallelLogger.Log($"[INFO] Extracting patch2R.cpk");
                if (File.Exists($@"{directory}\patch2R.cpk"))
                    CriFsUnpack($@"{directory}\patch2R.cpk", pathToExtract, patch2RFiles);
                else
                    Utilities.ParallelLogger.Log($"[ERROR] Couldn't find patch2R.cpk in {directory}.");
                if (language != "English")
                {
                    var patchSuffix = String.Empty;
                    switch (language)
                    {
                        case "French":
                            patchSuffix = "_F";
                            break;
                        case "Italian":
                            patchSuffix = "_I";
                            break;
                        case "German":
                            patchSuffix = "_G";
                            break;
                        case "Spanish":
                            patchSuffix = "_S";
                            break;
                    }
                    string[] patch2RLocalizedFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_patch2R{patchSuffix}.csv");
                    Utilities.ParallelLogger.Log($"[INFO] Extracting patch2R{patchSuffix}.cpk");
                    if (File.Exists($@"{directory}\patch2R{patchSuffix}.cpk"))
                        CriFsUnpack($@"{directory}\patch2R{patchSuffix}.cpk", pathToExtract, patch2RLocalizedFiles);
                    else
                        Utilities.ParallelLogger.Log($"[ERROR] Couldn't find patch2R{patchSuffix}.cpk in {directory}.");
                }
            }

            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P5R switch
        public static async Task UnpackP5RSwitchCPKs(string directory, string language)
        {
            if (!Directory.Exists(directory))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {directory}. Please correct the file path.");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P5Rswitch}";
            Directory.CreateDirectory(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Extracting PATCH1.CPK");
            if (File.Exists($@"{directory}\PATCH1.CPK"))
                CriFsUnpack($@"{directory}\PATCH1.CPK", pathToExtract);
            else
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find PATCH1.CPK in {directory}.");

            Utilities.ParallelLogger.Log($"[INFO] Extracting ALL_USEU.CPK (This will take awhile)");
            if (File.Exists($@"{directory}\ALL_USEU.CPK"))
                CriFsUnpack($@"{directory}\ALL_USEU.CPK", pathToExtract);
            else
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find ALL_USEU.CPK in {directory}.");

            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // P4G Vita
        public static async Task UnpackP4GCPK(string cpk)
        {
            if (!File.Exists(cpk))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {cpk}. Please correct the file path.");
                return;
            }
            if (!File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_p4gdata.csv"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find CSV file used for unpacking in Dependencies\FilteredCpkCsv");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.P4Gvita}";
            Directory.CreateDirectory(pathToExtract);

            if (!File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_p4gdata.csv"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find CSV file used for unpacking in Dependencies\FilteredCpkCsv");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
                return;
            }

            string[] dataFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_p4gdata.csv");

            Utilities.ParallelLogger.Log($"[INFO] Extracting data.cpk");
            CriFsUnpack(cpk, pathToExtract, dataFiles);

            Utilities.ParallelLogger.Log("[INFO] Unpacking extracted files");
            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // PQ2
        public static async Task UnpackPQ2CPK(string cpk)
        {
            if (!File.Exists(cpk))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {cpk}. Please correct the file path.");
                return;
            }
            if (!File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_data_pq2.csv"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find CSV file used for unpacking in Dependencies\FilteredCpkCsv");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.PQ2}";
            Directory.CreateDirectory(pathToExtract);

            string[] dataFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_data_pq2.csv");

            Utilities.ParallelLogger.Log($"[INFO] Extracting data.cpk");
            CriFsUnpack(cpk, pathToExtract, dataFiles);

            Utilities.ParallelLogger.Log("[INFO] Unpacking extracted files");
            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        // PQ
        public static async Task UnpackPQCPK(string cpk)
        {
            if (!File.Exists(cpk))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Couldn't find {cpk}. Please correct the file path.");
                return;
            }
            if (!File.Exists($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_data_pq.csv"))
            {
                Utilities.ParallelLogger.Log($@"[ERROR] Couldn't find CSV file used for unpacking in Dependencies\FilteredCpkCsv");
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            string pathToExtract = $@"{Folders.Original}\{Games.PQ}";
            Directory.CreateDirectory(pathToExtract);

            string[] dataFiles = File.ReadAllLines($@"{Folders.Dependencies}\FilteredCpkCsv\filtered_data_pq.csv");

            Utilities.ParallelLogger.Log($"[INFO] Extracting data.cpk");
            CriFsUnpack(cpk, pathToExtract, dataFiles);

            Utilities.ParallelLogger.Log("[INFO] Unpacking extracted files");
            ExtractWantedFiles(pathToExtract);

            Utilities.ParallelLogger.Log($"[INFO] Finished unpacking base files!");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        private static void CriFsUnpack(string cpk, string dir, string[] fileList = null)
        {
            using var fileStream = new FileStream(cpk, FileMode.Open);
            using var reader = CriFsLib.Instance.CreateCpkReader(fileStream, true);
            var files = reader.GetFiles();
            fileStream.Close();

            bool extractAll = fileList == null;
            using var extractor = CriFsLib.Instance.CreateBatchExtractor<FileToExtract>(cpk, P5RCrypto.DecryptionFunction);
            for (int x = 0; x < files.Length; x++)
            {
                string filePath = string.IsNullOrEmpty(files[x].Directory) ? files[x].FileName : $@"{files[x].Directory}/{files[x].FileName}";

                if (extractAll || fileList.Contains(filePath))
                {
                    extractor.QueueItem(new FileToExtract(Path.Combine(dir, filePath), files[x]));
                    Utilities.ParallelLogger.Log($@"[INFO] Extracting {filePath}");
                }
            }

            extractor.WaitForCompletion();
            ArrayRental.Reset();
        }
        private static void ExtractWantedFiles(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            // we want to extract binMerger.containerExtenions + gsd + tpc
            var extensionsToExtract = new HashSet<string>(binMerge.containerExtensions, StringComparer.OrdinalIgnoreCase)
            {
                ".gsd", ".tpc"
            };
            var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(file => extensionsToExtract.Contains(Path.GetExtension(file)));

            var wantedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".bf", ".bmd", ".pm1", ".dat", ".ctd", ".ftd", ".spd", ".acb", ".awb"
            };
            foreach (string file in files)
            {
                List<string> contents = PAKUtils.GetFileContents(file);
                // i don't know why but there are invalid pak files in some games 
                if (contents == null)
                    continue;

                // Check if there are any files we want (or files that could have files we want) and unpack them if so
                bool containersFound = contents.Exists(x => binMerge.containerExtensions.Contains(Path.GetExtension(file)));

                if (contents.Exists(x => wantedExtensions.Contains(Path.GetExtension(x)) || containersFound))
                {
                    Utilities.ParallelLogger.Log($"[INFO] Unpacking {file}");
                    PAKUtils.Unpack(file);

                    // Search the location of the unpacked container for wanted files
                    if (containersFound)
                        ExtractWantedFiles(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)));
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

    }
}
