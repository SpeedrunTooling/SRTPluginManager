using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using SRTPluginManager.Core;
using static SRTPluginManager.Core.Utilities;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Windows.Media;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for PluginView.xaml
    /// </summary>
    public partial class PluginView : UserControl
    {
        private VersionType HasDotNetCore = VersionType.None;
        private VersionType HasDotNetCore32 = VersionType.None;
        private Plugins CurrentPlugin = Plugins.SRTPluginProviderRE1C;
        public PluginConfiguration config;
        public RadioButton[] PluginSelection;

        private Process SRTProcess;

        private bool SRTUpdated = false;
        private bool SRTInstalled = false;
        private bool CurrentPluginUpdated = false;
        private bool CurrentPluginInstalled = false;

        public PluginView()
        {
            InitializeComponent();

            PluginSelection = new RadioButton[] {
                ResidentEvil1,
                ResidentEvil1HD,
                ResidentEvil2,
                ResidentEvil2Remake,
                ResidentEvil3,
                ResidentEvil3Remake,
                ResidentEvil4,
                ResidentEvil5,
                ResidentEvil6,
                ResidentEvil7,
                ResidentEvil8
            };

            config = LoadConfiguration<PluginConfiguration>();
            InitSRTData();
            GetPluginVersions(false);
            GetCurrentPluginData();
        }

        private void InitSRTData()
        {
            CheckDotNet();
            SRTCurrentRelease.Text = GetFileVersionInfo(ApplicationPath, "SRTHost64.exe");
            SRTLatestRelease.Text = GetHostVersion(false).ToString();
            SRTInstalled = SRTCurrentRelease.Text != "0.0.0.0";
            SRTUpdated = IsUpdated(SRTCurrentRelease.Text, SRTLatestRelease.Text);
        }

        private void Update()
        {
            UpdateHost();
            UpdatePlugins();
            UpdateDotNet();
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

        private void UpdatePlugins()
        {
            var i = 0;
            foreach (PluginInfo info in config.PluginConfig)
            {
                if (info.currentVersion == "0.0.0.0")
                {
                    PluginSelection[i].Visibility = Visibility.Collapsed;
                }
                else
                {
                    PluginSelection[i].Visibility = Visibility.Visible;
                }
                i++;
            }
        }

        private void UpdateDotNet()
        {
            if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.Optional)
            {
                DotNetCore.Visibility = Visibility.Collapsed;
                return;
            }
            else if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.Required)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Collapsed;
                Download32.Visibility = Visibility.Visible;
                Download32.Content = "Optional 32Bit";
                return;
            }
            else if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.None)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Collapsed;
                Download32.Visibility = Visibility.Visible;
                return;
            }
            else if (HasDotNetCore == VersionType.None && HasDotNetCore32 == VersionType.None)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Visible;
                Download32.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Visible;
                Download32.Visibility = Visibility.Visible;
                return;
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
                if (!SRTInstalled)
                {
                    SRTGetUpdate.Content = "Install";
                    SRTGetUpdate.Visibility = Visibility.Visible;
                }
                else if (SRTInstalled && !SRTUpdated)
                {
                    SRTGetUpdate.Content = "Update";
                    SRTGetUpdate.Visibility = Visibility.Visible;
                    StartSRTHost.Visibility = Visibility.Visible;
                }

                if (!CurrentPluginInstalled)
                {
                    GetUpdate.Content = "Install";
                    GetUpdate.Visibility = Visibility.Visible;
                    UpdateProgressBar.SetValue(Grid.RowProperty, 2);
                    UpdateProgressBar.Text = "Not Installed";
                    UpdateProgressBar.Visibility = Visibility.Visible;
                }
                else if (CurrentPluginInstalled && !CurrentPluginUpdated)
                {
                    GetUpdate.Content = "Update";
                    GetUpdate.Visibility = Visibility.Visible;
                    UpdateProgressBar.SetValue(Grid.RowProperty, 2);
                    UpdateProgressBar.Text = "Update Pending";
                    UpdateProgressBar.Visibility = Visibility.Visible;
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
            CurrentPlugin = GetCurrentSelectedPlugin();
            SetData(config.PluginConfig[(int)CurrentPlugin]);
            Update();
        }

        private Plugins GetCurrentSelectedPlugin()
        {
            if (ResidentEvil1HD.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Visible;
                User3.ImageSource = new ImageSourceConverter().ConvertFromString(toastSource) as ImageSource;
                Username3.Text = "Cursed Toast";
                return Plugins.SRTPluginProviderRE1;
            }
            else if (ResidentEvil2.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Collapsed;
                //User3.ImageSource = new ImageSourceConverter().ConvertFromString(willowSource) as ImageSource;
                //Username3.Text = "WillowTheWhisperSR";
                return Plugins.SRTPluginProviderRE2C;
            }
            else if (ResidentEvil2Remake.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Visible;
                User3.ImageSource = new ImageSourceConverter().ConvertFromString(toastSource) as ImageSource;
                Username3.Text = "Cursed Toast";
                return Plugins.SRTPluginProviderRE2;
            }
            else if (ResidentEvil3.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Collapsed;
                //User3.ImageSource = new ImageSourceConverter().ConvertFromString(willowSource) as ImageSource;
                //Username3.Text = "WillowTheWhisperSR";
                return Plugins.SRTPluginProviderRE3C;
            }
            else if (ResidentEvil3Remake.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Visible;
                User3.ImageSource = new ImageSourceConverter().ConvertFromString(toastSource) as ImageSource;
                Username3.Text = "Cursed Toast";
                return Plugins.SRTPluginProviderRE3;
            }
            else if (ResidentEvil4.IsChecked == true)
            {
                return Plugins.SRTPluginProviderRE4;
            }
            else if (ResidentEvil5.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(mysterionSource) as ImageSource;
                Username1.Text = "Mysterion06";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Collapsed;
                //User3.ImageSource = new ImageSourceConverter().ConvertFromString(willowSource) as ImageSource;
                //Username3.Text = "WillowTheWhisperSR";
                return Plugins.SRTPluginProviderRE5;
            }
            else if (ResidentEvil6.IsChecked == true)
            {
                return Plugins.SRTPluginProviderRE6;
            }
            else if (ResidentEvil7.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Collapsed;
                //User3.ImageSource = new ImageSourceConverter().ConvertFromString(willowSource) as ImageSource;
                //Username3.Text = "WillowTheWhisperSR";
                return Plugins.SRTPluginProviderRE7;
            }
            else if (ResidentEvil8.IsChecked == true)
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(squirrelSource) as ImageSource;
                Username1.Text = "Squirrelies";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Collapsed;
                //User3.ImageSource = new ImageSourceConverter().ConvertFromString(willowSource) as ImageSource;
                //Username3.Text = "WillowTheWhisperSR";
                return Plugins.SRTPluginProviderRE8;
            }
            else
            {
                User1.ImageSource = new ImageSourceConverter().ConvertFromString(mysterionSource) as ImageSource;
                Username1.Text = "Mysterion06";
                UserPanel2.Visibility = Visibility.Visible;
                User2.ImageSource = new ImageSourceConverter().ConvertFromString(vgrSource) as ImageSource;
                Username2.Text = "VideoGameRoulette";
                UserPanel3.Visibility = Visibility.Collapsed;
                //User3.ImageSource = new ImageSourceConverter().ConvertFromString(willowSource) as ImageSource;
                //Username3.Text = "WillowTheWhisperSR";
                return Plugins.SRTPluginProviderRE1C;
            }
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
            if (!pluginInfo.hasPluginProvider)
            {
                GetUpdate.Visibility = Visibility.Collapsed;
                StartSRTHost.Visibility = Visibility.Collapsed;
                UpdateProgressBar.Text = "No Plugin Available";
            }
            else
            {
                VersionCheck(CurrentRelease.Text, LatestRelease.Text);
            }
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
            await DownloadFile(CurrentPlugin.ToString() + ".zip", config.PluginConfig[(int)CurrentPlugin].downloadURL, GetUpdate, PluginFolderPath);
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
            SRTProcess?.Kill();
            var psi = new ProcessStartInfo(filePath, string.Format("--Provider={0}", Application.Current.Dispatcher.Invoke(() => PluginName.Text)));
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            //psi.WindowStyle = ProcessWindowStyle.Normal; //might be useful?
            SRTProcess = new Process();
            SRTProcess.StartInfo = psi;
            SRTProcess.EnableRaisingEvents = true;
            SRTProcess.OutputDataReceived += P_OutputDataReceived;
            SRTProcess.ErrorDataReceived += P_OutputDataReceived;
            SRTProcess.Start();
            SRTProcess.BeginOutputReadLine();
            SRTProcess.BeginErrorReadLine();
            await Task.Run(() =>
            {
                SRTProcess.WaitForExit();
            });
        }

        private void CloseSRT(object sender, RoutedEventArgs e)
        {
            if (SRTProcess != null)
            {
                SRTProcess.StandardInput.AutoFlush = true;
                SRTProcess.StandardInput.Write("\x3");
                SRTProcess.StandardInput.Flush();
                SRTProcess.StandardInput.Close();
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
            switch (CurrentPlugin)
            {
                case Plugins.SRTPluginProviderRE1:
                case Plugins.SRTPluginProviderRE1C:
                case Plugins.SRTPluginProviderRE2C:
                case Plugins.SRTPluginProviderRE3C:
                case Plugins.SRTPluginProviderRE4:
                case Plugins.SRTPluginProviderRE5:
                    return GetSRT(Platform.x86);
                case Plugins.SRTPluginProviderRE2:
                case Plugins.SRTPluginProviderRE3:
                case Plugins.SRTPluginProviderRE6:
                case Plugins.SRTPluginProviderRE7:
                case Plugins.SRTPluginProviderRE8:
                default:
                    return GetSRT(Platform.x64);
            }
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
        }

        public void Log(string line)
        {
            ConsoleBox.AppendText(string.Format("{0}\r\n", line));
        }

        public void Log(string[] lines)
        {
            foreach (string line in lines)
            {
                Log(line);
            }
        }

        private void CheckDotNet()
        {
            HasDotNetCore = CheckDotNetCore(16);
            HasDotNetCore32 = CheckDotNetCore(15, 16);
            Update();
        }

        private void SRTGetUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (HasDotNetCore == VersionType.None || HasDotNetCore32 == VersionType.None)
            {
                var result = MessageBox.Show(".Net Core 3.1 Required! Please download both 32bit and 64bit", "Warning");
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }
            DownloadFile("SRTHost.zip", "https://neonblu.com/SRT/Host/SRTHost_2440-Beta-Signed-Release.7z", GetUpdate, ApplicationPath);
        }

        private void Download64_Click(object sender, RoutedEventArgs e)
        {
            DownloadFile("dotNet64.exe", dotNetCore64URL, Download64);
        }

        private void Download32_Click(object sender, RoutedEventArgs e)
        {
            DownloadFile("dotNet32.exe", dotNetCore32URL, Download32);
        }
    }
}
