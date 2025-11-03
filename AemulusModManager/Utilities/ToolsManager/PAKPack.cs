using AtlusFileSystemLibrary.Common.IO;
using AtlusFileSystemLibrary.FileSystems.PAK;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
internal abstract class AddOrReplaceCommand : ICommand
{
    protected static bool Execute(string[] args, bool allowAdd)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Expected at least 2 arguments.");
            return false;
        }

        var inputPath = args[1];
        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Input file doesn't exist.");
            return false;
        }

        if (!PAKFileSystem.TryOpen(inputPath, out var pak))
        {
            Console.WriteLine("Invalid PAK file.");
            return false;
        }

        string outputPath = inputPath;

        if (Directory.Exists(args[2]))
        {
            var directoryPath = args[2];

            if (args.Length > 3)
            {
                outputPath = args[3];
            }

            using (pak)
            {
                foreach (string file in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
                {
                    Console.WriteLine($"{(pak.Exists(file) ? "Replacing" : "Adding")} {file}");
                    pak.AddFile(file.Substring(directoryPath.Length)
                                     .Trim(Path.DirectorySeparatorChar)
                                     .Replace("\\", "/"),
                                 file, ConflictPolicy.Replace);
                }

                Console.WriteLine("Saving...");
                pak.Save(outputPath);
            }
        }
        else
        {
            if (args.Length > 4)
            {
                outputPath = args[4];
            }

            using (pak)
            {
                var entryName = args[2];
                var entryExists = pak.Exists(entryName);

                if (!allowAdd && !entryExists)
                {
                    Console.WriteLine("Specified entry doesn't exist.");
                    return false;
                }

                var filePath = args[3];
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Specified replacement file doesn't exist.");
                    return false;
                }

                Console.WriteLine($"{(entryExists ? "Replacing" : "Adding")} {entryName}");
                pak.AddFile(entryName, filePath, ConflictPolicy.Replace);

                Console.WriteLine("Saving...");
                pak.Save(outputPath);
            }
        }

        return true;
    }
*/

namespace AemulusModManager.Utilities.ToolsManager
{
    // Code adapted from AtlusFileSystemLibrary/PAKPack by tge-was-taken
    // https://github.com/tge-was-taken/AtlusFileSystemLibrary/
    public static class PAKPack
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

        public static void Replace(string pakInputPath, string pathToFile, string outputPath = null)
        {
            outputPath ??= pakInputPath;
            Directory.CreateDirectory(outputPath);

        }
    }
}
