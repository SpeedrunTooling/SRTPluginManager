using SRTPluginManager.Core;
using SRTPluginManager.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SRTPluginBase.Extensions;
using static SRTPluginManager.Core.Utilities;

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for WidgetView.xaml
    /// </summary>
    public partial class WidgetView : UserControl
    {
        private List<string> paramList = new List<string>();
        private List<string> paramListInventory = new List<string>();
        public PluginConfiguration config;
        public RadioButton[] PluginSelection;

        public WidgetView()
        {
            InitializeComponent();
            DataContext = new ComboBoxViewModel();

            AuthToken.Password = GetSetting("UUID").ToString();

            config = LoadConfiguration<PluginConfiguration>();
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
                paramListInventory.Remove("separated");
            }

            // DISPLAY PLAYER 2
            if ((bool)Player2Check.IsChecked)
            {
                if (!paramList.Contains("isplayer2"))
                {
                    paramList.Add("isplayer2");
                    paramListInventory.Add("isplayer2");
                }
                if (!(bool)SeparatePlayerData.IsChecked)
                {
                    var results = MessageBox.Show("Is Sheva Param can only be used if players are separated. Would you like to separate them now?", "Error Players Not Separated", MessageBoxButton.YesNo);
                    if (results == MessageBoxResult.Yes)
                    {
                        SeparatePlayerData.IsChecked = true;
                    }
                    else
                    {
                        Player2Check.IsChecked = false;
                        if (paramList.Contains("isplayer2"))
                        {
                            paramList.Remove("isplayer2");
                            paramListInventory.Remove("isplayer2");
                        }
                    }
                }
            }
            else
            {
                paramList.Remove("isplayer2");
                paramListInventory.Remove("isplayer2");
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

            if (TestBlock.Text != "None")
            {
                defaultLink += string.Format("&name={0}", RE6NameBox.SelectedIndex);
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
                wsc.SaveConfiguration(WebSocketConfig);
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

        private void RE6NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TestBlock.Text = RE6NameBox.SelectedItem.ToString();
            GetParams();
        }

        private void TestBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RE6NameBox.IsDropDownOpen = true;
        }
    }
}
