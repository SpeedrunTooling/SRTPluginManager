using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace SRTPluginManager.Core
{
    public class UpdateService
    {
        private string currentVersion = "1.0.0.9"; // Replace with actual version retrieval

        public async Task CheckForUpdatesAsync()
        {
            string json = await GetUpdateMetadataAsync();
            if (json != null)
            {
                var updateInfo = ParseUpdateMetadata(json);
                if (IsUpdateAvailable(currentVersion, updateInfo.latestVersion))
                {
                    NotifyUserOfUpdate(updateInfo);
                }
            }
        }

        private async Task<string> GetUpdateMetadataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync("https://github.com/SuperTrainerM/SuperTrainer-Scripts/releases/download/1.0.0.0/Update.json");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to check for updates. " + ex.Message);
                    return null;
                }
            }
        }

        private UpdateInfo ParseUpdateMetadata(string json)
        {
            return JsonConvert.DeserializeObject<UpdateInfo>(json);
        }

        private bool IsUpdateAvailable(string currentVersion, string latestVersion)
        {
            return string.Compare(currentVersion, latestVersion, StringComparison.Ordinal) < 0;
        }

        private void NotifyUserOfUpdate(UpdateInfo updateInfo)
        {
            var result = MessageBox.Show(
                $"A new version ({updateInfo.latestVersion}) is available. Do you want to update now?\n\n{updateInfo.releaseNotes}",
                "Update Available",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information
            );

            if (result == MessageBoxResult.Yes)
            {
                StartUpdate(updateInfo.downloadUrl);
            }
        }

        private async void StartUpdate(string downloadUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetByteArrayAsync(downloadUrl);
                    var tempFile = Path.GetTempFileName();
                    File.WriteAllBytes(tempFile, response);

                    // Open the downloaded file (assuming it's an installer or update package)
                    Process.Start(tempFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to download update. " + ex.Message);
                }
            }
        }
    }

    public class UpdateInfo
    {
        public string latestVersion { get; set; }
        public string downloadUrl { get; set; }
        public string releaseNotes { get; set; }
    }
}
