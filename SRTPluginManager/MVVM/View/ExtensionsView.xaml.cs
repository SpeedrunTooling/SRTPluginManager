using SRTPluginManager.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for ExtensionsView.xaml
    /// </summary>
    public partial class ExtensionsView : UserControl
    {
        private Extensions CurrentExtension = Extensions.SRTPluginUIJSON;

        private enum Extensions
        {
            SRTPluginUIJSON,
            SRTPluginWebsocket
        }

        public ExtensionsView()
        {
            InitializeComponent();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            bool results = Utilities.IsUpdatedTimestamp("ExtensionUpdate");
            if (results)
            {
                GetExtensionVersions(false);
            }
            GetCurrentPluginData();
        }

        private void GetCurrentPluginData()
        {
            CurrentExtension = GetCurrentSelectedPlugin();
            SetData(Config.ExtensionsConfig[(int)CurrentExtension]);
        }

        private Extensions GetCurrentSelectedPlugin()
        {
            if (WebSocket.IsChecked == true) { return Extensions.SRTPluginWebsocket; }
            return Extensions.SRTPluginUIJSON;
        }

        private void SetData(PluginInfo pluginInfo)
        {
            var dllPath = Path.Combine(ExtensionFolderPath, pluginInfo.pluginName);
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
            if (!pluginInfo.hasPluginProvider)
            {
                InstallUpdate.Visibility = Visibility.Collapsed;
                Uninstall.Visibility = Visibility.Collapsed;
                //UpdateProgressBar.Text = "No Plugin Available";
            }
            else
            {
                VersionCheck(CurrentRelease.Text, LatestRelease.Text);
            }
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
            await DownloadFile(CurrentExtension.ToString() + ".zip", Config.ExtensionsConfig[(int)CurrentExtension].downloadURL, InstallUpdate, ExtensionFolderPath);
            await Task.Run(() =>
            {
                autoResetEvent.WaitOne();
            });
            GetExtensionData();
        }

        private void GetExtensionData()
        {
            CurrentExtension = GetCurrentSelectedExtension();
            SetData(Config.ExtensionsConfig[(int)CurrentExtension]);
        }

        private Extensions GetCurrentSelectedExtension()
        {
            if (WebSocket.IsChecked == true) { return Extensions.SRTPluginWebsocket; }
            return Extensions.SRTPluginUIJSON;
        }

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            UninstallExtension(ExtensionFolderPath, Config.ExtensionsConfig[(int)CurrentExtension].pluginName);
        }

        private void UpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentPluginData();
        }
    }
}
