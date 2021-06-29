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

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for PluginView.xaml
    /// </summary>
    public partial class PluginView : UserControl
    {
        private Plugins CurrentPlugin = Plugins.SRTPluginProviderRE1C;
        private PluginInfo[] ProviderInfo;
        public PluginConfiguration config;
        public RadioButton[] PluginSelection;

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

            ProviderInfo = new PluginInfo[11];
            for (var i = 0; i < ProviderInfo.Length; i++)
            {
                ProviderInfo[i] = new PluginInfo();
            }

            config = LoadConfiguration<PluginConfiguration>();
        }

        #region Config
        //private string GetConfigFile(Assembly a) => Path.Combine(new FileInfo(a.Location).DirectoryName, string.Format("{0}.cfg", Path.GetFileNameWithoutExtension(new FileInfo(a.Location).Name)));
        //private JsonSerializerOptions jso = new JsonSerializerOptions() { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };
        //public virtual T LoadConfiguration<T>() where T : class, new() => LoadConfiguration<T>(GetConfigFile(Assembly.GetCallingAssembly()));
        //private T LoadConfiguration<T>(string configFile) where T : class, new()
        //{
        //    try
        //    {
        //        FileInfo configFileInfo = new FileInfo(configFile);
        //        if (configFileInfo.Exists)
        //            using (FileStream fs = new FileStream(configFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
        //                return JsonSerializer.DeserializeAsync<T>(fs, jso).Result;
        //        else
        //            return new T(); // File did not exist, just return a new instance.
        //    }
        //    catch
        //    {
        //        return new T(); // An exception occurred when reading the file, return a new instance.
        //    }
        //}
        //
        //public virtual void SaveConfiguration<T>(T configuration) where T : class, new() => SaveConfiguration<T>(configuration, GetConfigFile(Assembly.GetCallingAssembly()));
        //private void SaveConfiguration<T>(T configuration, string configFile) where T : class, new()
        //{
        //    if (configuration != null) // Only save if configuration is not null.
        //    {
        //        try
        //        {
        //            using (FileStream fs = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
        //                JsonSerializer.SerializeAsync<T>(fs, configuration, jso).Wait();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}
        #endregion

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            GetPluginVersions(false);
            GetCurrentPluginData();
        }

        private void Update()
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
                SetCurrent.Visibility = Visibility.Collapsed;
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
            if (current != "0.0.0.0" && latest != "0.0.0.0")
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
            else if (current == "0.0.0.0")
            {
                SetCurrent.Visibility = Visibility.Collapsed;
                GetUpdate.Visibility = Visibility.Visible;
                GetUpdate.Content = "Install";
                UpdateProgressBar.Text = "Not Installed";
                return;
            }
            SetCurrent.Visibility = Visibility.Visible;
            GetUpdate.Visibility = Visibility.Collapsed;
            UpdateProgressBar.Text = "Update To Date";
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
        }

        private void ResidentEvil1HD_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil2_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil2Remake_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil3_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil3Remake_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil4_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil5_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil6_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil7_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void ResidentEvil8_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }

        private void SetCurrent_Click(object sender, RoutedEventArgs e)
        {
            ReplacePluginProvider();
            // Write Startup Routine
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        private async void GetUpdate_Click(object sender, RoutedEventArgs e)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            DownloadPlugin(CurrentPlugin.ToString() + ".zip", config.PluginConfig[(int)CurrentPlugin].downloadURL, GetUpdate, ProviderFolderPath);
            GetCurrentPluginData();
        }

        //private async Task UpdateManager()
        //{
        //    return Task.Run(() =>
        //    {
        //        DownloadPlugin(CurrentPlugin.ToString() + ".zip", config.PluginConfig[(int)CurrentPlugin].downloadURL, GetUpdate, ProviderFolderPath);
        //        while ()
        //        {
        //
        //        }
        //    });
        //}
    }
}
