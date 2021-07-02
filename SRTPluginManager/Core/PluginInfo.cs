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
        public string tagURL => "https://api.github.com/repos/SpeedrunTooling/" + pluginName + "/releases/latest";
        public bool hasPluginProvider => currentVersion != "0.0.0.0";
        public string downloadURL => hasPluginProvider ? "https://github.com/SpeedrunTooling/" + pluginName + "/releases/download/" + currentVersion + "/" + pluginName + "-v" + currentVersion + ".zip" : null;
        public PluginInfo()
        {
            currentVersion = "0.0.0.0";
        }
    }
}
