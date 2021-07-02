using SRTPluginManager.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for WidgetView.xaml
    /// </summary>
    public partial class WidgetView : UserControl
    {
        private Plugins CurrentPlugin;
        private List<string> paramList = new List<string>();
        private List<string> paramListInventory = new List<string>();
        private string GameName;
        public PluginConfiguration config;
        public RadioButton[] PluginSelection;

        public WidgetView()
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

            AuthToken.Password = GetSetting("UUID").ToString();
            CurrentPlugin = GetCurrentSelectedPlugin();
            GameName = CurrentPlugin.ToString();

            config = LoadConfiguration<PluginConfiguration>();
            Update();
        }

        private void Update()
        {
            CurrentPlugin = GetCurrentSelectedPlugin();
            GameName = CurrentPlugin.ToString();
            UpdatePlugins();
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
            GetParams();
        }

        public void GetParams(object sender, RoutedEventArgs e)
        {
            GetParams();
        }

        public void GetParams()
        {
            string defaultLink = "https://speedruntooling.github.io/StatsHUD";
            string inventoryLink = "https://speedruntooling.github.io/InventoryHUD";

            if (AuthToken.Password != "")
            {
                defaultLink += string.Format("?token={0}", AuthToken.Password);
                inventoryLink += string.Format("?token={0}", AuthToken.Password);
            }
            // HIDE IGT
            if ((bool)HideIGT.IsChecked)
            {
                if (!paramList.Contains("hideigt"))
                {
                    paramList.Add("hideigt");
                }
            }
            else
            {
                paramList.Remove("hideigt");
            }

            // HIDE MONEY
            if ((bool)HideMoney.IsChecked)
            {
                if (!paramList.Contains("hidemoney"))
                {
                    paramList.Add("hidemoney");
                }
            }
            else
            {
                paramList.Remove("hidemoney");
            }

            // HIDE DA
            if ((bool)HideDA.IsChecked)
            {
                if (!paramList.Contains("hideda"))
                {
                    paramList.Add("hideda");
                }
            }
            else
            {
                paramList.Remove("hideda");
            }

            // HIDE MISC STATS
            if ((bool)HideStats.IsChecked)
            {
                if (!paramList.Contains("hidestats"))
                {
                    paramList.Add("hidestats");
                }
            }
            else
            {
                paramList.Remove("hidestats");
            }

            // SEPARATE PLAYER STATS
            if ((bool)SeparatePlayerData.IsChecked)
            {
                if (!paramList.Contains("separated"))
                {
                    paramList.Add("separated");
                    paramListInventory.Add("separated");
                }
            }
            else
            {
                paramList.Remove("separated");
                paramListInventory.Add("separated");
            }

            // DISPLAY PLAYER 2
            if ((bool)Player2Check.IsChecked)
            {
                if (!paramList.Contains("isplayer2"))
                {
                    paramList.Add("isplayer2");
                    paramListInventory.Add("isplayer2");
                }
            }
            else
            {
                paramList.Remove("isplayer2");
                paramListInventory.Add("isplayer2");
            }

            // SHOW BOSS ONLY
            if ((bool)BossOnly.IsChecked)
            {
                if (!paramList.Contains("bossonly"))
                {
                    paramList.Add("bossonly");
                }
            }
            else
            {
                paramList.Remove("bossonly");
            }

            // HIDE ALL ENEMIES
            if ((bool)HideEnemies.IsChecked)
            {
                if (!paramList.Contains("enemy"))
                {
                    paramList.Add("enemy");
                }
            }
            else
            {
                paramList.Remove("enemy");
            }

            // HIDE DEBUG INFO
            if ((bool)HideDebugInfo.IsChecked)
            {
                if (!paramList.Contains("debug"))
                {
                    paramList.Add("debug");
                }
            }
            else
            {
                paramList.Remove("debug");
            }

            // SET PARAMS STATS HUD
            for (var i = 0; i < paramList.Count; i++)
            {
                defaultLink += string.Format("&{0}=1", paramList[i].ToString());
            }

            // SET PARAMS INVENTORY HUD
            for (var i = 0; i < paramListInventory.Count; i++)
            {
                inventoryLink += string.Format("&{0}=1", paramListInventory[i].ToString());
            }

            WebURL.Text = defaultLink;
            InventoryURL.Text = inventoryLink;
        }

        public void GenerateAuthToken(object sender, RoutedEventArgs e)
        {
            var result = Guid.NewGuid().ToString("B").ToUpper();
            var uuid = result.Trim(new char[] { '{', '}' });
            AuthToken.Password = uuid;
            UpdateSetting("UUID", uuid);
            if (File.Exists(WebSocketConfig))
            {
                var wsc = LoadConfiguration<WebsocketConfiguration>(WebSocketConfig);
                wsc.Username = uuid;
                SaveConfiguration(wsc, WebSocketConfig);
            }
            else
            {
                MessageBox.Show("Error: SRTPluginWebSocket is not installed. Please switch to Extensions tab install.", "Error Extension Missing");
            }
            GetParams();
        }

        public void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(WebURL.Text);
        }
    }
}
