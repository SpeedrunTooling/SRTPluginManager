using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            var current = GetFileVersionInfo(AppContext.BaseDirectory, "SRTPluginManager.exe");
            var latest = Config.ManagerConfig.currentVersion;
            var results = IsUpdated(current, latest);
            if (results)
                InstallUpdate.Visibility = Visibility.Collapsed;
            else
                InstallUpdate.Visibility = Visibility.Visible;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
                DragMove();
            }
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeToTaskbar(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private async void InstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            await DownloadManagerAsync("ManagerUpdate.zip", Config.ManagerConfig.downloadURL, InstallUpdate, TempFolderPath);
            Process.Start(Path.Combine(TempFolderPath, "SRTPluginManager.exe"), "--LoadUpdate");
            Environment.Exit(0);
        }
    }
}
