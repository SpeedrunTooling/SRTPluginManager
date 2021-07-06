using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {

        public HomeView()
        {
            InitializeComponent();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            GetDirectory(TempFolderPath);
            GetDirectory(PluginFolderPath);
        }

        private void GetDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }
            Directory.CreateDirectory(path);
        }

        private void Github(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            var url = GetGithubURL(button.Name);
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "chrome";
            p.StartInfo.Arguments = url;
            p.Start();
        }

        private void Twitch(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            var url = GetTwitchURL(button.Name);
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "chrome";
            p.StartInfo.Arguments = url;
            p.Start();
        }

        private string GetGithubURL(string name)
        {
            switch (name)
            {
                case "Github1":
                    return @"https://github.com/Squirrelies";
                case "Github2":
                    return @"https://github.com/VideoGameRoulette";
                case "Github3":
                    return @"https://github.com/CursedToast";
                case "Github4":
                    return @"https://github.com/markrawls";
                case "Github5":
                    return @"https://github.com/sychotixdev";
                default:
                    return null;
            }
        }

        private string GetTwitchURL(string name)
        {
            switch (name)
            {
                case "Twitch1":
                    return @"https://www.twitch.tv/Squirrelies";
                case "Twitch2":
                    return @"https://www.twitch.tv/VideoGameRoulette";
                case "Twitch3":
                    return @"https://www.twitch.tv/CursedToast";
                case "Twitch4":
                    return @"https://www.twitch.tv/WillowTheWhisperSR";
                case "Twitch5":
                    return @"https://www.twitch.tv/";
                default:
                    return null;
            }
        }

        
    }
}
