using System;
using System.IO;
using System.Reflection;
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
            GetDirectory(ProviderFolderPath);
            GetDirectory(ExtensionFolderPath);
        }

        private void GetDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }
            Directory.CreateDirectory(path);
        }
    }
}
