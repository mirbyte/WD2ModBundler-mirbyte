using System;
using System.IO;
using System.Text.Json;

namespace WD2ModBundler.Helpers
{
    public sealed class PathSettings
    {
        public string? SevenZipPath { get; set; }
        public string? ModFolderPath { get; set; }
        public string? PatchToolsPath { get; set; }

        private static string FilePath => Path.Combine(AppContext.BaseDirectory, "settings.json");

        public static PathSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new PathSettings();

                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<PathSettings>(json) ?? new PathSettings();
            }
            catch (Exception)
            {
                return new PathSettings();
            }
        }

        public static void Update(Action<PathSettings> update)
        {
            PathSettings settings = Load();
            update(settings);
            settings.Save();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
