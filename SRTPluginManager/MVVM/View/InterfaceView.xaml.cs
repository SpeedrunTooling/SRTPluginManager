using SRTPluginManager.Core;
using System;
using System.Diagnostics;
using System.IO;
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

        private void GetCurrentPluginData()
        {
            SetData(Config.InterfaceConfig[(int)CurrentInterface]);
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
            await DownloadFileAsync(CurrentInterface.ToString() + ".zip", Config.InterfaceConfig[CurrentInterface].downloadURL, InstallUpdate, PluginFolderPath);
            await Task.Run(() =>
            {
                autoResetEvent.WaitOne();
            });
            GetExtensionData();
        }

        private void GetExtensionData()
        {
            SetData(Config.InterfaceConfig[CurrentInterface]);
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

    }
}
