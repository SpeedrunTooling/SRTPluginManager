using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static SRTPluginManager.Core.Utilities;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private const string updateJsonUrl = "https://github.com/SpeedrunTooling/SRTPluginManager/releases/download/1.0.0.0U/Update.json";

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

                    if (IsNewVersionAvailable(App.currentVersion, latestVersion))
                    {
                        MessageBox.Show(
                            $"A new version {latestVersion} is available.\n\nRelease Notes:\n{releaseNotes}\nPress 'Check for Updates' to update your Manager",
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

        private bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            Version current = new Version(currentVersion);
            Version latest = new Version(latestVersion);
            return latest > current;
        }
    }
}
