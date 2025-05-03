using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRTPluginManager.TrainerHelper
{
    public static class ConfigManager
    {
        public static async Task<List<GameIndexEntry>> LoadGameIndex(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
                Console.WriteLine($"Loaded index JSON: {json}");
                return JsonSerializer.Deserialize<List<GameIndexEntry>>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading game index: {ex.Message}");
            }
        }

        public static async Task<GameConfig> LoadGameConfig(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
                Console.WriteLine($"Loaded game JSON: {json}");
                return JsonSerializer.Deserialize<GameConfig>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading game configuration: {ex.Message}");
            }
        }
    }

    public class GameIndexEntry
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Creator { get; set; }
    }

    public class GameConfig
    {
        public string Name { get; set; }
        public string ProcessName { get; set; }
        public List<GameOption> Options { get; set; }
    }

    public class GameOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
    }
}
