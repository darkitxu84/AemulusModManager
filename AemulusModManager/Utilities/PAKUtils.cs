using AtlusFileSystemLibrary.Common.IO;
using AtlusFileSystemLibrary.FileSystems.PAK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AemulusModManager.Utilities
{
    // Code adapted from AtlusFileSystemLibrary/PAKPack by tge-was-taken
    // https://github.com/tge-was-taken/AtlusFileSystemLibrary/
    public static class PAKUtils
    {
        private static bool TryGetValidPak(string path, out PAKFileSystem pak)
        {
            pak = null;

            if (!File.Exists(path))
            {
                ParallelLogger.Log($"[ERROR] {path} file does not exist.");
                return false;
            }
            if (!PAKFileSystem.TryOpen(path, out pak))
            {
                ParallelLogger.Log($"[ERROR] {path} is an invalid PAK file.");
                return false;
            }   

            return true;
        }

        public static List<string> GetFileContents(string path)
        {
            if (!TryGetValidPak(path, out var pak))
            {
                ParallelLogger.Log($"[ERROR] Could not get file contents of {path}");
                return null;
            }
            using (pak) 
            {
                var enumeratedFiles = pak.EnumerateFiles().ToList();
                return enumeratedFiles;
            }
        }

        public static void Unpack(string inputPath, string outputPath = null)
        {
            outputPath ??= Path.ChangeExtension(inputPath, null);
            Directory.CreateDirectory(outputPath);

            if (!TryGetValidPak(inputPath, out var pak))
            {
                ParallelLogger.Log($"[ERROR] Could not unpack file: {inputPath}");
                return;
            }
            using (pak)
            {
                foreach (string file in pak.EnumerateFiles())
                {
                    var normalizedFilePath = file.Replace("../", "");
                    using var stream = FileUtils.Create(outputPath + Path.DirectorySeparatorChar + normalizedFilePath);
                    using var inputStream = pak.OpenFile(file);
                    inputStream.CopyTo(stream);
                }
            }
        }

        public static void Replace(string pakInputPath, string unk, string outputPath = null)
        {
            outputPath ??= pakInputPath;
            Directory.CreateDirectory(outputPath);

        }
    }
}
