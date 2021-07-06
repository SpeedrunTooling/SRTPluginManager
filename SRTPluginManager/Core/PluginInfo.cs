using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRTPluginManager.Core
{
    public class PluginInfo
    {
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string tagURL { get; set; }
        public string downloadURL { get; set; }
        public PluginInfo()
        {
        }
    }
}
