using System.Diagnostics;
using System.Windows.Controls;

namespace SRTPluginManager.MVVM.View
{
    public partial class ContactUsView: UserControl
    {
        public ContactUsView()
        {
            InitializeComponent();
        }

        private void DiscordButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/3FGp24vWcg",
                UseShellExecute = true
            });
        }

        private void TwitterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://x.com/SpeedRunTool",
                UseShellExecute = true
            });
        }

        private void GithubButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/SpeedrunTooling",
                UseShellExecute = true
            });
        }
    }
}
