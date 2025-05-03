using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace SRTPluginManager.MVVM.View
{
    public partial class UpdatesView : UserControl
    {
        private const string updateJsonUrl = "https://github.com/SpeedrunTooling/SRTPluginManager/releases/download/1.0.0.0U/Update.json";
        private const string currentVersion = "1.0.1.1"; // Ensure this matches your actual current version

        public UpdatesView()
        {
            InitializeComponent();
        }

        private async void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string updateJson = await client.GetStringAsync(updateJsonUrl);
                    if (string.IsNullOrWhiteSpace(updateJson))
                    {
                        MessageBox.Show("Update information is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    JObject updateData = JObject.Parse(updateJson);
                    string latestVersion = updateData["latestVersion"]?.ToString();
                    string downloadUrl = updateData["downloadUrl"]?.ToString();
                    string releaseNotes = updateData["releaseNotes"]?.ToString();

                    if (string.IsNullOrEmpty(latestVersion) || string.IsNullOrEmpty(downloadUrl))
                    {
                        MessageBox.Show("Update information is incomplete or missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (IsNewVersionAvailable(currentVersion, latestVersion))
                    {
                        MessageBoxResult result = MessageBox.Show(
                            $"A new version {latestVersion} is available.\n\nRelease Notes:\n{releaseNotes}\n\nDo you want to update?",
                            "Update Available",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Close the application and start the update process
                            await CloseAndUpdate(downloadUrl);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are already using the latest version.", "No Updates Available", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            Version current = new Version(currentVersion);
            Version latest = new Version(latestVersion);
            return latest > current;
        }

        private async Task CloseAndUpdate(string downloadUrl)
        {
            try
            {
                // Create a temporary file path for the ZIP file
                string tempZipPath = Path.Combine(Path.GetTempPath(), "update.zip");

                // Download the update file
                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(downloadUrl);
                    File.WriteAllBytes(tempZipPath, fileBytes);
                }

                // Create a batch file path
                string batchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.bat");

                // Write the batch file to the application's directory
                File.WriteAllText(batchFilePath,
                $@"@echo off
                timeout /t 10 /nobreak
                set ""tempZipPath={tempZipPath}""
                set ""appDirectory={AppDomain.CurrentDomain.BaseDirectory}""
                powershell -Command ""Expand-Archive -Path '%tempZipPath%' -DestinationPath '%appDirectory%' -Force""
                del ""%tempZipPath%""
                start "" "" ""%appDirectory%SRTPluginManager.exe""");

                // Close the application
                Application.Current.Shutdown();

                // Start the batch file to handle the update
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batchFilePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update the application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
