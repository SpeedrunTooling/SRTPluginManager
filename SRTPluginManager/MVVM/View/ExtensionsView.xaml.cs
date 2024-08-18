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
    /// Interaction logic for ExtensionsView.xaml
    /// </summary>
    public partial class ExtensionsView : UserControl
    {
        private int CurrentExtension = 0;

        private RadioButton[] Extensions = new RadioButton[32];
        private TextBlock[] Contributors = new TextBlock[8];
 
        public ExtensionsView()
        {
            InitializeComponent();

            // INIT RADIO BUTTONS
            for (var i = 0; i < Extensions.Length; i++)
            {
                Extensions[i] = new RadioButton();
            }

            // INIT TEXTBLOCKS
            for (var j = 0; j < Contributors.Length; j++)
            {
                Contributors[j] = new TextBlock();
            }

            GetExtensions();
            GetCurrentPluginData();
        }

        private void GetExtensions()
        {
            var i = 0;
            
            foreach (PluginInfo pi in Config.ExtensionsConfig)
            {
                // ADD EXTENSION PLUGIN LIST ITEMS - PLUGINS
                Extensions[i].Name = string.Format("ID_{0}", i);
                Extensions[i].Content = pi.internalName;
                Extensions[i].IsChecked = i == 0;
                Extensions[i].Style = (Style)Application.Current.Resources["PluginButtonTheme"];
                Extensions[i].Click += UpdateInfo_Click;
                ExtensionList.Children.Add(Extensions[i]);
                i++;
            }
        }

        private void GetContributors()
        {
            ContributorList.Children.Clear();
            var i = 0;
            foreach (string name in Config.ExtensionsConfig[CurrentExtension].contributors)
            {
                Contributors[i].Name = string.Format("TID_{0}", i);
                Contributors[i].Margin = new Thickness(30, 10, 0, 10);
                Contributors[i].FontSize = 24;
                Contributors[i].Text = name;
                ContributorList.Children.Add(Contributors[i]);
                i++;
            }
        }

        private void GetCurrentPluginData()
        {
            SetData(Config.ExtensionsConfig[(int)CurrentExtension]);
            GetContributors();
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
            await DownloadFileAsync(Config.ExtensionsConfig[CurrentExtension].pluginName, Config.ExtensionsConfig[CurrentExtension].pluginName + ".zip", Config.ExtensionsConfig[CurrentExtension].downloadURL, InstallUpdate, PluginFolderPath, false);
            await Task.Run(() =>
            {
                autoResetEvent.WaitOne();
            });
            GetExtensionData();
        }

        private void GetExtensionData()
        {
            SetData(Config.ExtensionsConfig[CurrentExtension]);
        }

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            UninstallExtension(PluginFolderPath, Config.ExtensionsConfig[CurrentExtension].pluginName);
        }

        private void UpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            RadioButton obj = (RadioButton)e.Source;
            var result = obj.Name.Split('_')[1];
            CurrentExtension = Int32.Parse(result);
            GetExtensionData();
            GetContributors();
        }
    }
}
