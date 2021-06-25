using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRTPluginManager.Core
{
    public class PluginInfo
    {
        public string pluginName = "";
        public string currentVersion = "";
        public bool hasPluginProvider => (currentVersion != "");
        public string tagURL => (hasPluginProvider) ? "https://api.github.com/repos/ResidentEvilSpeedrunning/" + pluginName + "/releases/latest" : null;
        public string downloadURL => (hasPluginProvider) ? "https://github.com/ResidentEvilSpeedrunning/" + pluginName + "/releases/download/" + currentVersion + "/" + pluginName + "-v" + currentVersion + ".zip" : null;
        public PluginInfo()
        {
        }
    }
}
