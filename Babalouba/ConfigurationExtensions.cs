using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Babalouba
{
    public static class ConfigurationExtensions
    {
        public static void Load(this Configuration self, string path)
        {
            try
            {
                var jsonString = File.ReadAllText(path);
                var temp = JsonSerializer.Deserialize<Configuration>(jsonString);
                self.DefaultTheme = temp.DefaultTheme;
                self.LibraryFolders = temp.LibraryFolders;
            }
            catch (Exception)
            {
                // this is fine
            }
            
            if (self.LibraryFolders == null || self.LibraryFolders.Count == 0)
            {
                self.LibraryFolders = new List<string>
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                };
            }
        }

        public static async Task Save(this Configuration config, string path)
        {
            await using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, config);
        }
    }
}