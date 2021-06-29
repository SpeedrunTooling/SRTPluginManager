using System;
using System.Collections.Generic;
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

namespace SRTPluginManager.MVVM.View
{
    /// <summary>
    /// Interaction logic for LogsView.xaml
    /// </summary>
    public partial class LogsView : UserControl
    {
        public LogsView()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            ConsoleBox.Text = "";
        }

        public void Log(string line)
        {
            ConsoleBox.Text += string.Format("{0}{1}", line, "&#10;");
        }

        public void Log(string[] lines)
        {
            foreach(string line in lines)
            {
                ConsoleBox.Text += string.Format("{0}{1}", line, "&#10;");
            }
        }
    }
}
