﻿using System;
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
            Update();
        }

        private void InitSRTData()
        {
            CheckDotNet();
            SRTCurrentRelease.Text = GetFileVersionInfo(ApplicationPath, "SRTHost64.dll");
            SRTLatestRelease.Text = GetHostVersion(false).ToString();
            SRTInstalled = SRTCurrentRelease.Text != "0.0.0.0";
            SRTUpdated = IsUpdated(SRTCurrentRelease.Text, SRTLatestRelease.Text);
            EnableJSON.IsChecked = (bool)GetSetting("UIJSONEnabled");
            EnableWebSocket.IsChecked = (bool)GetSetting("WebSocketEnabled");
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Update()
        {
            UpdateHost();
            UpdatePlugins();
            UpdateDotNet();
            UpdateSRTState();
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
                PluginBox.SetValue(Grid.ColumnSpanProperty, 2);
                return;
            }
            else if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.Required)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Collapsed;
                Download32.Visibility = Visibility.Visible;
                Download32.Content = "Download Optional 32Bit Update";
                PluginBox.SetValue(Grid.ColumnSpanProperty, 1);
                return;
            }
            else if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.None)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Collapsed;
                Download32.Visibility = Visibility.Visible;
                PluginBox.SetValue(Grid.ColumnSpanProperty, 1);
                return;
            }
            else if (HasDotNetCore == VersionType.None && HasDotNetCore32 == VersionType.None)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Visible;
                Download32.Visibility = Visibility.Collapsed;
                PluginBox.SetValue(Grid.ColumnSpanProperty, 1);
                return;
            }
            else
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Visible;
                Download32.Visibility = Visibility.Visible;
                PluginBox.SetValue(Grid.ColumnSpanProperty, 1);
                return;
            }
        }

        private void UpdateSRTState()
        {
            if (SRTInstalled && CurrentPluginInstalled)
            {
                StartSRTHost.Visibility = Visibility.Visible;

                if (SRTUpdated) SRTGetUpdate.Visibility = Visibility.Collapsed;
                else
                {
                    SRTGetUpdate.Content = "Update";
                    SRTGetUpdate.Visibility = Visibility.Visible;
                }

                if (CurrentPluginUpdated) GetUpdate.Visibility = Visibility.Collapsed;
                else
                {
                    GetUpdate.Content = "Update";
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
            if (ResidentEvil1HD.IsChecked == true) { return Plugins.SRTPluginProviderRE1; }
            else if (ResidentEvil2.IsChecked == true) { return Plugins.SRTPluginProviderRE2C; }
            else if (ResidentEvil2Remake.IsChecked == true) { return Plugins.SRTPluginProviderRE2; }
            else if (ResidentEvil3.IsChecked == true) { return Plugins.SRTPluginProviderRE3C; }
            else if (ResidentEvil3Remake.IsChecked == true) { return Plugins.SRTPluginProviderRE3; }
            else if (ResidentEvil4.IsChecked == true) { return Plugins.SRTPluginProviderRE4; }
            else if (ResidentEvil5.IsChecked == true) { return Plugins.SRTPluginProviderRE5; }
            else if (ResidentEvil6.IsChecked == true) { return Plugins.SRTPluginProviderRE6; }
            else if (ResidentEvil7.IsChecked == true) { return Plugins.SRTPluginProviderRE7; }
            else if (ResidentEvil8.IsChecked == true) { return Plugins.SRTPluginProviderRE8; }
            return Plugins.SRTPluginProviderRE1C;
        }

        private void SetData(PluginInfo pluginInfo)
        {
            var dllPath = Path.Combine(ProviderFolderPath, pluginInfo.pluginName);
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
            else if (current != "0.0.0.0" && latest != "0.0.0.0")
            {
                var currentSplit = current.Split('.');
                var latestSplit = latest.Split('.');

                for (var i = 0; i < 4; i++)
                {
                    if (currentSplit[i] != latestSplit[i])
                    {
                        CurrentPluginUpdated = false;
                        CurrentPluginInstalled = true;
                        Update();
                        return;
                    }
                }
            }
            CurrentPluginUpdated = true;
            CurrentPluginInstalled = true;
            Update();
        }

        private void ReplacePluginProvider()
        {
            // Removes Current Plugin Installed
            var dirs = Directory.GetDirectories(PluginFolderPath);
            foreach (string directory in dirs)
            {
                if (directory.Contains("PluginProvider"))
                {
                    Directory.Delete(directory, true);
                }
            }

            // Adds New Plugin Installed To Current
            foreach (string dir in Directory.GetDirectories(ProviderFolderPath))
            {
                if (Path.GetFileName(dir) == config.PluginConfig[(int)CurrentPlugin].pluginName)
                {
                    CopyPluginProvider(dir, PluginFolderPath);
                }
            }
        }

        private void ResidentEvil1_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil1HD_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil2_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil2Remake_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil3_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil3Remake_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil4_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil5_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil6_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil7_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
            Update();
        }

        private void ResidentEvil8_Click(object sender, RoutedEventArgs e)
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
            await DownloadFile(CurrentPlugin.ToString() + ".zip", config.PluginConfig[(int)CurrentPlugin].downloadURL, GetUpdate, ProviderFolderPath);
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
            await Task.Run(() =>
            {
                var p = Process.Start(filePath, PluginName.Text);
                while (p.Responding)
                {
                    //RUN SRT
                }
            });
        }

        private string GetPlatform()
        {
            switch(CurrentPlugin)
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
                    return "SRTHost32.exe";
                default:
                    return "SRTHost64.exe";
            }
        }

        public void ClearLog()
        {
            ConsoleBox.Text = "";
        }

        public void Log(string line)
        {
            ConsoleBox.Text += string.Format("{0}{1}", line, "&#10;");
        }

        public void Log(string[] lines)
        {
            foreach (string line in lines)
            {
                ConsoleBox.Text += string.Format("{0}{1}", line, "&#10;");
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

        private void GetExtensions(object sender, RoutedEventArgs e)
        {
            var sourceJSON = Path.Combine(ExtensionFolderPath, "SRTPluginUIJSON");
            var sourceWS = Path.Combine(ExtensionFolderPath, "SRTPluginWebSocket");
            if ((bool)EnableJSON.IsChecked)
            {
                if (Directory.Exists(sourceJSON)) GetExtensionsSelected((bool)EnableJSON.IsChecked, (bool)EnableWebSocket.IsChecked);
                else
                {
                    EnableJSON.IsChecked = false;
                    MessageBox.Show("SRTPluginUIJSON Not Installed.", "Error Missing Plugin");
                }
            }
            else if ((bool)EnableWebSocket.IsChecked)
            {
                if (Directory.Exists(sourceJSON)) GetExtensionsSelected((bool)EnableJSON.IsChecked, (bool)EnableWebSocket.IsChecked);
                else
                {
                    EnableWebSocket.IsChecked = false;
                    MessageBox.Show("SRTPluginWebSocket Not Installed.", "Error Missing Plugin");
                }
            }
        }

    }
}
