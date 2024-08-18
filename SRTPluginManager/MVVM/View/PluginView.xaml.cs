using SRTPluginManager.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for PluginView.xaml
    /// </summary>
    public partial class PluginView : UserControl
    {
        private int CurrentPlugin = 0;

        private RadioButton[] Plugins = new RadioButton[32];
        private TextBlock[] Contributors = new TextBlock[8];

        private Process SRTProcess;

        private bool SRTUpdated = false;
        private bool SRTInstalled = false;
        private bool CurrentPluginUpdated = false;
        private bool CurrentPluginInstalled = false;

        public PluginView()
        {
            InitializeComponent();
            // INIT RADIO BUTTONS
            for (var i = 0; i < Plugins.Length; i++)
            {
                Plugins[i] = new RadioButton();
            }

            // INIT TEXTBLOCKS
            for (var j = 0; j < Contributors.Length; j++)
            {
                Contributors[j] = new TextBlock();
            }
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
            await UpdateConfig();
            GetPlugins();
            InitSRTData();
            GetCurrentPluginData();
            await UpdateDownloadCount(); // New Method Call
        }

        private async Task UpdateDownloadCount()
        {
            string pluginName = Config.PluginConfig[CurrentPlugin].pluginName;

            // Extract the repository path from the download URL
            string downloadUrl = Config.PluginConfig[CurrentPlugin].downloadURL;
            string repoInfo = ExtractRepoInfoFromDownloadUrl(downloadUrl);

            // Construct the shields.io URL
            string url = $"https://img.shields.io/github/downloads/{repoInfo}/total?color=%23007EC6&style=for-the-badge";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string svgContent = await client.GetStringAsync(url);
                    string downloadCount = ParseDownloadCountFromSVG(svgContent);
                    Dispatcher.Invoke(() =>
                    {
                        DownloadCount.Text = $"Total Downloads: {downloadCount}";
                    });
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network errors)
                    Dispatcher.Invoke(() =>
                    {
                        DownloadCount.Text = "Failed to load download count.";
                    });
                }
            }
        }

        private string ExtractRepoInfoFromDownloadUrl(string downloadUrl)
        {
            // Example download URL: https://github.com/SpeedrunTooling/SRTPluginProviderED/releases/download/1.0.0.2/SRTPluginProviderED.zip
            var uri = new Uri(downloadUrl);
            var segments = uri.Segments;

            if (segments.Length >= 3)
            {
                // Combine the organization/user name and the repository name
                string orgName = segments[1].TrimEnd('/');
                string repoName = segments[2].TrimEnd('/');
                return $"{orgName}/{repoName}";
            }

            return string.Empty; // Return empty if the URL format is unexpected
        }

        private string ParseDownloadCountFromSVG(string svgContent)
        {
            // Find the section where the download count is located
            var startIndex = svgContent.IndexOf("font-weight=\"bold\">") + "font-weight=\"bold\">".Length;
            var endIndex = svgContent.IndexOf("</text>", startIndex);

            if (startIndex > 0 && endIndex > startIndex)
            {
                return svgContent.Substring(startIndex, endIndex - startIndex).Trim();
            }

            return "N/A"; // Return "N/A" if the count cannot be found
        }




        private void GetPlugins()
        {
            var i = 0;

            foreach (PluginInfo pi in Config.PluginConfig)
            {
                // ADD EXTENSION PLUGIN LIST ITEMS - PLUGINS
                Plugins[i].Name = string.Format("ID_{0}", i);
                Plugins[i].Content = pi.internalName;
                Plugins[i].IsChecked = i == 0;
                Plugins[i].Style = (Style)Application.Current.Resources["PluginButtonTheme"];
                Plugins[i].Click += ListSelection_Click;
                PluginList.Children.Add(Plugins[i]);
                i++;
            }
        }

        private void GetContributors()
        {
            ContributorList.Children.Clear();
            var i = 0;
            foreach (string name in Config.PluginConfig[CurrentPlugin].contributors)
            {
                Contributors[i].Name = string.Format("TID_{0}", i);
                Contributors[i].Margin = new Thickness(30, 10, 0, 10);
                Contributors[i].FontSize = 24;
                Contributors[i].Text = name;
                ContributorList.Children.Add(Contributors[i]);
                i++;
            }
        }

        private void InitSRTData()
        {
            SRTCurrentRelease.Text = GetFileVersionInfo(ApplicationPath, "SRTHost64.exe");
            SRTLatestRelease.Text = Config.SRTConfig.currentVersion;
            SRTInstalled = SRTCurrentRelease.Text != "0.0.0.0";
            SRTUpdated = IsUpdated(SRTCurrentRelease.Text, SRTLatestRelease.Text);
            Update();
        }

        private void Update()
        {
            UpdateHost();
            UpdateSRTState();
            GetPlatform();
        }

        private void UpdateHost()
        {
            if (SRTUpdated)
            {
                SRTGetUpdate.Visibility = Visibility.Collapsed;
            }
            else
            {
                SRTGetUpdate.Visibility = Visibility.Visible;
            }
        }

        private void UpdateSRTState()
        {
            if (SRTInstalled && CurrentPluginInstalled)
            {
                StartSRTHost.Visibility = Visibility.Visible;
                StopSRTHost.Visibility = Visibility.Visible;

                if (SRTUpdated) SRTGetUpdate.Visibility = Visibility.Collapsed;
                else
                {
                    SRTGetUpdate.Content = "Update";
                    SRTGetUpdate.Visibility = Visibility.Visible;
                    StopSRTHost.Visibility = Visibility.Visible;
                }

                if (CurrentPluginUpdated) 
                {
                    UpdateProgressBar.Text = "Up to Date";
                    GetUpdate.Visibility = Visibility.Collapsed; 
                }
                else
                {
                    GetUpdate.Content = "Update";
                    UpdateProgressBar.Text = "Update Pending";
                    GetUpdate.Visibility = Visibility.Visible;
                }
            }
            else
            {
                StartSRTHost.Visibility = Visibility.Collapsed;
                StopSRTHost.Visibility = Visibility.Collapsed;
                if (!SRTInstalled)
                {
                    SRTGetUpdate.Content = "Install";
                    SRTGetUpdate.Visibility = Visibility.Visible;
                }
                else if (SRTInstalled && !SRTUpdated)
                {
                    SRTGetUpdate.Content = "Update";
                    SRTGetUpdate.Visibility = Visibility.Visible;
                }

                if (!CurrentPluginInstalled)
                {
                    GetUpdate.Content = "Install";
                    GetUpdate.Visibility = Visibility.Visible;
                    UpdateProgressBar.Visibility = Visibility.Collapsed;
                }
                else if (CurrentPluginInstalled && !CurrentPluginUpdated)
                {
                    GetUpdate.Content = "Update";
                    GetUpdate.Visibility = Visibility.Visible;
                    UpdateProgressBar.SetValue(Grid.RowProperty, 2);
                    UpdateProgressBar.Text = "Update Pending";
                    UpdateProgressBar.Visibility = Visibility.Visible;
                    StartSRTHost.Visibility = Visibility.Visible;
                    StopSRTHost.Visibility = Visibility.Visible;
                }
                else
                {
                    GetUpdate.Visibility = Visibility.Collapsed;
                    UpdateProgressBar.SetValue(Grid.RowProperty, 1);
                    UpdateProgressBar.Text = "Up To Date";
                    UpdateProgressBar.Visibility = Visibility.Visible;
                }
            }
        }

        private void GetCurrentPluginData()
        {
            SetData(Config.PluginConfig[CurrentPlugin]);
            GetContributors();
            Update();
            UpdateDownloadCount(); // New Method Call
        }


        private void SetData(PluginInfo pluginInfo)
        {
            var dllPath = Path.Combine(PluginFolderPath, pluginInfo.pluginName);
            PluginName.Text = pluginInfo.pluginName + ".dll";
            var filePath = Path.Combine(dllPath, PluginName.Text);
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
            Update();
        }

        private void VersionCheck(string current, string latest)
        {
            if (current == "0.0.0.0")
            {
                CurrentPluginUpdated = false;
                CurrentPluginInstalled = false;
                Update();
                return;
            }
            var updated = IsUpdated(current, latest);
            if (updated)
            {
                CurrentPluginUpdated = true;
                CurrentPluginInstalled = true;
                Update();
                return;
            }
            CurrentPluginUpdated = false;
            CurrentPluginInstalled = true;
            Update();
        }

        private void ListSelection_Click(object sender, RoutedEventArgs e)
        {
            RadioButton obj = (RadioButton)e.Source;
            var result = obj.Name.Split('_')[1];
            CurrentPlugin = Int32.Parse(result);
            GetCurrentPluginData();
            Update();
        }

        private void StartSRTHost_Click(object sender, RoutedEventArgs e)
        {
            //ReplacePluginProvider();
            // Write Startup Routine
            RunSRT();
        }

        private async void GetUpdate_Click(object sender, RoutedEventArgs e)
        {
            await DownloadFileAsync(Config.PluginConfig[(int)CurrentPlugin].pluginName, Config.PluginConfig[(int)CurrentPlugin].pluginName + ".zip", Config.PluginConfig[(int)CurrentPlugin].downloadURL, GetUpdate, PluginFolderPath, false);
            await Task.Run(() =>
            {
                autoResetEvent.WaitOne();
            });
            GetCurrentPluginData();
        }

        private async void RunSRT()
        {
            var filePath = Path.Combine(ApplicationPath, GetPlatform());
            var fileExists = File.Exists(filePath);
            if (!fileExists) { ConsoleBox.Text = "SRT Not Installed."; return; }
            ClearLog();
            KillSRT();
            var providerName = PluginName.Text.Split('.')[0];
            var psi = new ProcessStartInfo(filePath, string.Format("--Provider={0}", Application.Current.Dispatcher.Invoke(() => providerName)));
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            SRTProcess = new Process();
            SRTProcess.StartInfo = psi;
            SRTProcess.EnableRaisingEvents = true;
            SRTProcess.OutputDataReceived += P_OutputDataReceived;
            SRTProcess.ErrorDataReceived += P_OutputDataReceived;
            SRTProcess.Start();
            SRTProcess.BeginOutputReadLine();
            SRTProcess.BeginErrorReadLine();
            await Task.Run(SRTProcess.WaitForExit);
        }

        private async void CloseSRT(object sender, RoutedEventArgs e)
        {
            if (SRTProcess != null)
            {
                await SRTProcess.SendCtrlCAsync();
            }
            else
            {
                KillSRT(); // If the process has not been tracked by us, try to just kill existing processes the spicy way.
            }
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Log(e.Data);
            });
            //throw new NotImplementedException();
        }

        private string GetPlatform()
        {
            return GetSRT((Platform)Config.PluginConfig[CurrentPlugin].platform);
        }

        private string GetSRT(Platform platform)
        {
            switch (platform)
            {
                case Platform.x86:
                    SRTName.Text = "SRTHost32.exe";
                    break;
                default:
                    SRTName.Text = "SRTHost64.exe";
                    break;
            }
            return SRTName.Text;
        }

        public void ClearLog()
        {
            ConsoleBox.Clear();
            string[] lines = {
                ":'######::'########::'########:'##::::'##::'#######:::'######::'########:",
                "'##... ##: ##.... ##:... ##..:: ##:::: ##:'##.... ##:'##... ##:... ##..::",
                " ##:::..:: ##:::: ##:::: ##:::: ##:::: ##: ##:::: ##: ##:::..::::: ##::::",
                ". ######:: ########::::: ##:::: #########: ##:::: ##:. ######::::: ##::::",
                ":..... ##: ##.. ##:::::: ##:::: ##.... ##: ##:::: ##::..... ##:::: ##::::",
                "'##::: ##: ##::. ##::::: ##:::: ##:::: ##: ##:::: ##:'##::: ##:::: ##::::",
                ". ######:: ##:::. ##:::: ##:::: ##:::: ##:. #######::. ######::::: ##::::",
                ":......:::..:::::..:::::..:::::..:::::..:::.......::::......::::::..:::::"
            };

            foreach (string line in lines)
            {
                ConsoleBox.AppendText(string.Format("{0}\r\n", line));
            }
        }

        public void Log(string line)
        {
            ConsoleBox.AppendText(string.Format("{0}\r\n", line));
            ConsoleBox.ScrollToLine(ConsoleBox.LineCount - 1);
        }

        public void Log(string[] lines)
        {
            foreach (string line in lines)
            {
                Log(line);
            }
        }

        private async void SRTGetUpdate_Click(object sender, RoutedEventArgs e)
        {
            SRTGetUpdate.Content = "Please Wait! Installing...";
            await DownloadFileAsync("SRTHost", "SRTHost.zip", Config.SRTConfig.downloadURL, GetUpdate, ApplicationPath, true);
            InitSRTData();
        }

    }
}
