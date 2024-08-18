using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static SRTPluginManager.Core.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq; // Ensure you have this for JSON parsing
using System.IO.Compression;
using System.Linq;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private const string updateJsonUrl = "https://github.com/SpeedrunTooling/SRTPluginManager/releases/download/1.0.0.0U/Update.json";
        private const string currentVersion = "1.0.0.9"; // Ensure this matches your actual current version

        public HomeView()
        {
            InitializeComponent();
            CheckForUpdatesOnStartup();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(TempFolderPath);
            Directory.CreateDirectory(PluginFolderPath);
        }

        private void Github(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            var url = GetGithubURL(button.Name);
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "chrome";
            p.StartInfo.Arguments = url;
            p.Start();
        }

        private void Twitch(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            var url = GetTwitchURL(button.Name);
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "chrome";
            p.StartInfo.Arguments = url;
            p.Start();
        }

        private string GetGithubURL(string name)
        {
            switch (name)
            {
                case "Github1":
                    return @"https://github.com/Squirrelies";
                case "Github2":
                    return @"https://github.com/VideoGameRoulette";
                case "Github3":
                    return @"https://github.com/CursedToast";
                case "Github4":
                    return @"https://github.com/markrawls";
                case "Github5":
                    return @"https://github.com/sychotixdev";
                default:
                    return null;
            }
        }

        private string GetTwitchURL(string name)
        {
            switch (name)
            {
                case "Twitch1":
                    return @"https://www.twitch.tv/Squirrelies";
                case "Twitch2":
                    return @"https://www.twitch.tv/VideoGameRoulette";
                case "Twitch3":
                    return @"https://www.twitch.tv/CursedToast";
                case "Twitch4":
                    return @"https://www.twitch.tv/WillowTheWhisperSR";
                case "Twitch5":
                    return @"https://www.twitch.tv/";
                default:
                    return null;
            }
        }

        private async void CheckForUpdatesOnStartup()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string updateJson = await client.GetStringAsync(updateJsonUrl);
                    if (string.IsNullOrWhiteSpace(updateJson))
                    {
                        return; // Do nothing if no update information is available
                    }

                    JObject updateData = JObject.Parse(updateJson);
                    string latestVersion = updateData["latestVersion"]?.ToString();
                    string downloadUrl = updateData["downloadUrl"]?.ToString();
                    string releaseNotes = updateData["releaseNotes"]?.ToString();

                    if (string.IsNullOrEmpty(latestVersion) || string.IsNullOrEmpty(downloadUrl))
                    {
                        return; // Do nothing if update information is incomplete or missing
                    }

                    if (IsNewVersionAvailable(currentVersion, latestVersion))
                    {
                        MessageBox.Show(
                            $"A new version {latestVersion} is available.\n\nRelease Notes:\n{releaseNotes}",
                            "Update Available",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
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


        private void ExtractZip(string zipFilePath, string destinationDirectory)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue;

                        string destinationPath = Path.Combine(destinationDirectory, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        entry.ExtractToFile(destinationPath, overwrite: true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to extract the update: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestartApplication()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(exePath);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart the application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
