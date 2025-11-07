using System.IO;

namespace AemulusModManager.Utilities
{
    internal static class PathUtils
    {
        public static void DeleteIfExists(string path)
        {
            bool isDirectory = Directory.Exists(path);

            if (isDirectory)
            {
                Utilities.ParallelLogger.Log($"[INFO] Deleting directory: {path}");
                Directory.Delete(path, true);
                return;
            }

            if (File.Exists(path))
            {
                Utilities.ParallelLogger.Log($"[INFO] Deleting file: {path}");
                File.Delete(path);
                return;
            }
        }
    }
}
