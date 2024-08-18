using SRTPluginManager.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for HostView.xaml
    /// </summary>
    public partial class InterfaceView : UserControl
    {
        private int CurrentInterface = 0;

        private RadioButton[] Interfaces = new RadioButton[32];
        private TextBlock[] Contributors = new TextBlock[8];

        public InterfaceView()
        {
            InitializeComponent();

            // INIT RADIO BUTTONS
            for (var i = 0; i < Interfaces.Length; i++)
            {
                Interfaces[i] = new RadioButton();
            }

            // INIT TEXTBLOCKS
            for (var j = 0; j < Contributors.Length; j++)
            {
                Contributors[j] = new TextBlock();
            }

            GetInterfaces();
            GetCurrentPluginData();
        }

        private void GetInterfaces()
        {
            var i = 0;

            foreach (PluginInfo pi in Config.InterfaceConfig)
            {
                // ADD EXTENSION PLUGIN LIST ITEMS - PLUGINS
                Interfaces[i].Name = string.Format("ID_{0}", i);
                Interfaces[i].Content = pi.internalName;
                Interfaces[i].IsChecked = i == 0;
                Interfaces[i].Style = (Style)Application.Current.Resources["PluginButtonTheme"];
                Interfaces[i].Click += UpdateInfo_Click;
                InterfaceList.Children.Add(Interfaces[i]);
                i++;
            }
        }

        private void GetContributors()
        {
            ContributorList.Children.Clear();
            var i = 0;
            foreach (string name in Config.InterfaceConfig[CurrentInterface].contributors)
            {
                Contributors[i].Name = string.Format("TID_{0}", i);
                Contributors[i].Margin = new Thickness(30, 10, 0, 10);
                Contributors[i].FontSize = 24;
                Contributors[i].Text = name;
                ContributorList.Children.Add(Contributors[i]);
                i++;
            }
        }

        private async Task UpdateDownloadCount()
        {
            string downloadUrl = Config.InterfaceConfig[CurrentInterface].downloadURL;
            string repoInfo = ExtractRepoInfoFromDownloadUrl(downloadUrl);
            string url = $"https://img.shields.io/github/downloads/{repoInfo}/total?color=%23007EC6&style=for-the-badge";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string svgContent = await client.GetStringAsync(url);
                    string downloadCount = ParseDownloadCountFromSVG(svgContent);
                    Dispatcher.Invoke(() =>
                    {
                        DownloadsTextBlock.Text = $"Total Downloads: {downloadCount}";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        DownloadsTextBlock.Text = "Failed to load download count.";
                    });
                }
            }
        }

        private string ExtractRepoInfoFromDownloadUrl(string downloadUrl)
        {
            var uri = new Uri(downloadUrl);
            var segments = uri.Segments;

            if (segments.Length >= 3)
            {
                string orgName = segments[1].TrimEnd('/');
                string repoName = segments[2].TrimEnd('/');
                return $"{orgName}/{repoName}";
            }

            return string.Empty;
        }

        private string ParseDownloadCountFromSVG(string svgContent)
        {
            var startIndex = svgContent.IndexOf("font-weight=\"bold\">") + "font-weight=\"bold\">".Length;
            var endIndex = svgContent.IndexOf("</text>", startIndex);

            if (startIndex > 0 && endIndex > startIndex)
            {
                return svgContent.Substring(startIndex, endIndex - startIndex).Trim();
            }

            return "N/A";
        }

        private void GetCurrentPluginData()
        {
            SetData(Config.InterfaceConfig[(int)CurrentInterface]);
            GetContributors();
            UpdateDownloadCount();
        }

        private void SetData(PluginInfo pluginInfo)
        {
            var dllPath = Path.Combine(PluginFolderPath, pluginInfo.pluginName);
            ExtensionName.Text = pluginInfo.pluginName + ".dll";
            var filePath = Path.Combine(dllPath, ExtensionName.Text);
            LatestRelease.Text = pluginInfo.currentVersion;
            if (Directory.Exists(dllPath))
                CurrentRelease.Text = FileVersionInfo.GetVersionInfo(filePath).FileVersion;
            else
                CurrentRelease.Text = "0.0.0.0";

            SetVisibility(pluginInfo);
        }

        private void SetVisibility(PluginInfo pluginInfo)
        {
            VersionCheck(CurrentRelease.Text, LatestRelease.Text);
        }

        private void VersionCheck(string current, string latest)
        {
            if (current == "0.0.0.0")
            {
                Uninstall.Visibility = Visibility.Collapsed;
                InstallUpdate.Visibility = Visibility.Visible;
                InstallUpdate.Content = "Install";
                return;
            }
            var updated = IsUpdated(current, latest);
            if (updated)
            {
                Uninstall.Visibility = Visibility.Visible;
                InstallUpdate.Visibility = Visibility.Collapsed;
                return;
            }
            Uninstall.Visibility = Visibility.Collapsed;
            InstallUpdate.Visibility = Visibility.Visible;
            InstallUpdate.Content = "Update";
        }

        private async void InstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            await DownloadFileAsync(Config.InterfaceConfig[CurrentInterface].pluginName, Config.InterfaceConfig[CurrentInterface].pluginName + ".zip", Config.InterfaceConfig[CurrentInterface].downloadURL, InstallUpdate, PluginFolderPath, false);
            await Task.Run(() =>
            {
                autoResetEvent.WaitOne();
            });
            GetExtensionData();
        }

        private void GetExtensionData()
        {
            SetData(Config.InterfaceConfig[CurrentInterface]);
            UpdateDownloadCount();  // Ensure download count is updated
        }

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            UninstallExtension(PluginFolderPath, Config.InterfaceConfig[CurrentInterface].pluginName);
        }

        private void UpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            RadioButton obj = (RadioButton)e.Source;
            var result = obj.Name.Split('_')[1];
            CurrentInterface = Int32.Parse(result);
            GetExtensionData();
            GetContributors();
        }

        private void OpenParentFolder(object sender, RoutedEventArgs e)
        {
            var dllPath = Path.Combine(PluginFolderPath, Config.InterfaceConfig[(int)CurrentInterface].pluginName);
            if (Directory.Exists(dllPath))
                Process.Start("explorer.exe", dllPath);
        }
    }
}
