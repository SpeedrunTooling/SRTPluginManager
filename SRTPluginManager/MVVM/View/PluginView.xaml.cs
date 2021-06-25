using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using SRTPluginManager.Core;
using SRTPluginManager.Properties;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for PluginView.xaml
    /// </summary>
    public partial class PluginView : UserControl
    {
        private static readonly string ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string TempFolderPath = Path.Combine(ApplicationPath, "tmp");
        private static readonly string PluginFolderPath = Path.Combine(ApplicationPath, "plugins");
        private static readonly string ProviderFolderPath = Path.Combine(ApplicationPath, "providers");

        private Plugins CurrentPlugin = Plugins.SRTPluginProviderRE1C;
        private PluginInfo[] ProviderInfo = new PluginInfo[11];
        private enum Plugins
        {
            SRTPluginProviderRE1C,
            SRTPluginProviderRE1,
            SRTPluginProviderRE2C,
            SRTPluginProviderRE2,
            SRTPluginProviderRE3C,
            SRTPluginProviderRE3,
            SRTPluginProviderRE4,
            SRTPluginProviderRE5,
            SRTPluginProviderRE6,
            SRTPluginProviderRE7,
            SRTPluginProviderRE8
        }

        public PluginView()
        {
            InitializeComponent();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var previousTime = Settings.Default["LastUpdate"];
            long currentTime = DateTime.UtcNow.Ticks;
            long elapsedTicks = currentTime - (long)previousTime;
            TimeSpan timeSpan = new TimeSpan(elapsedTicks);

            if (timeSpan.TotalSeconds >= 3600)
            {
                Settings.Default["LastUpdate"] = currentTime;
                Settings.Default.Save();
                SetDefaultPluginInfo();
            }
            else
            {
                // READ STORED DATA
            }
        }

        private void SetDefaultPluginInfo()
        {
            string[] names = { "SRTPluginProviderRE1C", "SRTPluginProviderRE1", "SRTPluginProviderRE2C", "SRTPluginProviderRE2", "SRTPluginProviderRE3C", "SRTPluginProviderRE3", "SRTPluginProviderRE4", "SRTPluginProviderRE5", "SRTPluginProviderRE6", "SRTPluginProviderRE7", "SRTPluginProviderRE8" };
            for (var i = 0; i < names.Length; i++)
            {
                ProviderInfo[i].pluginName = names[i];
                ProviderInfo[i].currentVersion = GetPluginVersion(ProviderInfo[i].tagURL);
            }   
        }

        private string GetPluginVersion(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "ResidentEvilSpeedrunning";
            HttpWebResponse response;

            response = (HttpWebResponse)request.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var def = new
                {
                    tag_name = ""
                };

                var info = JsonConvert.DeserializeAnonymousType(result, def);
                return info.tag_name;
            }
        }

        private void GetCurrentPluginData()
        {
            CurrentPlugin = GetCurrentSelectedPlugin();
            SetData(ProviderInfo[(int)CurrentPlugin]);
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
            LatestRelease.Text = pluginInfo.currentVersion;
            if (Directory.Exists(dllPath)) 
                CurrentRelease.Text = FileVersionInfo.GetVersionInfo(dllPath).FileVersion;
            else 
                CurrentRelease.Text = "Not Installed";
        }

        private void SetVisibility(PluginInfo pluginInfo)
        {
            if (!pluginInfo.hasPluginProvider)
            {
                GetUpdate.Visibility = Visibility.Collapsed;
                SetCurrent.Visibility = Visibility.Collapsed;
                UpdateProgressBar.Text = "No Plugin Available";
            }
            else
            {
                VersionCheck(CurrentRelease.Text, LatestRelease.Text);
            }
        }

        private void VersionCheck(string current, string latest)
        {
            if (current != "Not Installed" && latest != "N/A")
            {
                var currentSplit = current.Split('.');
                var latestSplit = latest.Split('.');

                for (var i = 0; i < 4; i++)
                {
                    if (currentSplit[i] != latestSplit[i])
                    {
                        SetCurrent.Visibility = Visibility.Collapsed;
                        GetUpdate.Visibility = Visibility.Visible;
                        GetUpdate.Content = "Update";
                        UpdateProgressBar.Text = "Update Pending";
                        return;
                    }
                }
            }
            else if (current == "Not Installed")
            {
                SetCurrent.Visibility = Visibility.Collapsed;
                GetUpdate.Visibility = Visibility.Visible;
                GetUpdate.Content = "Install";
                UpdateProgressBar.Text = "Plugin Not Installed";
                return;
            }
            SetCurrent.Visibility = Visibility.Visible;
            GetUpdate.Visibility = Visibility.Collapsed;
            UpdateProgressBar.Text = "Update To Date";
        }

        private async void UnzipPackage(string name, string file, string destination)
        {
            var pluginPath = Path.Combine(TempFolderPath, name);
            if (!Directory.Exists(pluginPath))
            {
                ZipFile.ExtractToDirectory(file, TempFolderPath);
            }
            var dirs = Directory.GetDirectories(TempFolderPath);
            UpdatePackage(dirs[0], ProviderFolderPath);
        }

        private void UpdatePackage(string source, string destination)
        {
            var args = source.Split('\\');
            var repo = args[args.Length - 1];
            var dest = Path.Combine(destination, repo);
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            var filesSource = Directory.GetFiles(source);
            foreach (string file in filesSource)
            {
                var args2 = file.Split('\\');
                var destFile = args2[args2.Length - 1];
                File.Copy(file, Path.Combine(dest, destFile), true);
            }
            DeleteTmpFiles();
        }

        private void DeleteTmpFiles()
        {
            DeleteDirectories(TempFolderPath);
            DeleteFiles(TempFolderPath);
        }

        private void DeleteDirectories(string source)
        {
            var dirs = Directory.GetDirectories(source);
            foreach (string directory in dirs)
            {
                var files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                Directory.Delete(directory);
            }
        }

        private void DeleteFiles(string source)
        {
            var files = Directory.GetFiles(source);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        private void ReplacePluginProvider()
        {
            // Removes Current Plugin Installed
            var dirs = Directory.GetDirectories(PluginFolderPath);
            foreach (string directory in dirs)
            {
                if (directory.Contains("PluginProvider"))
                {
                    var files = Directory.GetFiles(directory);
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(directory);
                }
            }

            // Adds New Plugin Installed To Current
            var dirs2 = Directory.GetDirectories(ProviderFolderPath);
            UpdatePackage(dirs2[0], PluginFolderPath);
        }

        private void ResidentEvil1_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil1HD_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        
        private void ResidentEvil2_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil2Remake_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil3_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil3Remake_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil4_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil5_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil6_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil7_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void ResidentEvil8_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGameInfo();
        }

        private void SetCurrent_Click(object sender, RoutedEventArgs e)
        {
            ReplacePluginProvider();
            // Write Startup Routine
        }

        private async void GetUpdate_Click(object sender, RoutedEventArgs e)
        {
            //var url = GetCurrentURL();
            //var fileName = url.Split('/')[5];
            //var fileExt = ".zip";
            //var file = fileName + fileExt;
            //var filePath = Path.Combine(TempFolderPath, file);
            //UnzipPackage(fileName, filePath, ProviderFolderPath);
            //using (var wc = new WebClient())
            //{
            //    wc.DownloadProgressChanged += (s, ev) =>
            //    {
            //        //UpdateProgressBar.Visibility = Visibility.Visible;
            //        UpdateProgressBar.Text = ev.ProgressPercentage.ToString();
            //    };
            //    wc.DownloadFileCompleted += (s, ev) =>
            //    {
            //        //UpdateProgressBar.Visibility = Visibility.Collapsed;
            //        UnzipPackage(fileName, filePath, ProviderFolderPath);
            //    };
            //    wc.DownloadFileAsync(new Uri(GetDownloadLink()), TempFolderPath);
            //}
        }
    }
}
