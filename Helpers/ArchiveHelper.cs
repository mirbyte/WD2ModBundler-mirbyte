using System;
using System.IO;

namespace WD2ModBundler.Helpers
{
    public static class ArchiveHelper
    {
        private static string? user7ZipPath = null;

        public static void Set7ZipPath(string path)
        {
            if (File.Exists(path))
                user7ZipPath = path;
            else
                throw new FileNotFoundException("Selected 7-Zip executable does not exist.");
        }

        public static string? TryFind7Zip()
        {
            if (!string.IsNullOrEmpty(user7ZipPath))
                return user7ZipPath;

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            string path1 = Path.Combine(programFiles, "7-Zip", "7z.exe");
            string path2 = Path.Combine(programFilesX86, "7-Zip", "7z.exe");

            if (File.Exists(path1))
                return path1;

            if (File.Exists(path2))
                return path2;

            string? pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(pathEnv))
                return null;

            foreach (string dir in pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                string trimmed = dir.Trim().Trim('"');
                if (trimmed.Length == 0)
                    continue;

                string candidate = Path.Combine(trimmed, "7z.exe");
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }

        public static string Find7Zip()
        {
            return TryFind7Zip()
                ?? throw new FileNotFoundException("7-Zip not found. Please select manually.");
        }
    }
}