using System.Windows;
using System.Windows.Controls;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for HostView.xaml
    /// </summary>
    public partial class HostView : UserControl
    {
        private VersionType HasDotNetCore = VersionType.None;
        private VersionType HasDotNetCore32 = VersionType.None;

        public HostView()
        {
            InitializeComponent();
            CheckDotNet();
            CurrentRelease.Text = GetFileVersionInfo(ApplicationPath, "SRTHost64.dll");
            LatestRelease.Text = GetHostVersion(false).ToString();
            bool result2 = IsUpdated(CurrentRelease.Text, LatestRelease.Text);
            if (result2)
            {
                GetUpdate.Visibility = Visibility.Collapsed;
            }
            else
            {
                GetUpdate.Visibility = Visibility.Visible;
            }
            EnableJSON.IsChecked = (bool)GetSetting("UIJSONEnabled");
            EnableWebSocket.IsChecked = (bool)GetSetting("WebSocketEnabled");
            Update();
        }

        private void CheckDotNet()
        {
            HasDotNetCore = CheckDotNetCore(16);
            HasDotNetCore32 = CheckDotNetCore(15, 16);
            Update();
        }

        private void Update()
        {
            if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.Optional)
            {
                DotNetCore.Visibility = Visibility.Collapsed;
            }
            else if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.Required)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Collapsed;
                Download32.Visibility = Visibility.Visible;
                Download32.Content = "Download Optional 32Bit Update";
            }
            else if (HasDotNetCore == VersionType.Required && HasDotNetCore32 == VersionType.None)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Collapsed;
                Download32.Visibility = Visibility.Visible;
            }
            else if (HasDotNetCore == VersionType.None && HasDotNetCore32 == VersionType.None)
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Visible;
                Download32.Visibility = Visibility.Collapsed;
            }
            else
            {
                DotNetCore.Visibility = Visibility.Visible;
                Download64.Visibility = Visibility.Visible;
                Download32.Visibility = Visibility.Visible;
            }
        }

        private void GetUpdate_Click(object sender, RoutedEventArgs e)
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
            GetExtensionsSelected((bool)EnableJSON.IsChecked, (bool)EnableWebSocket.IsChecked);
        }

    }
}
