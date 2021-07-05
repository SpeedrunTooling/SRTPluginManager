using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRTPluginManager.Core
{
    public class SRTInfo
    {
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string tagURL => "https://api.github.com/repos/SpeedrunTooling/" + pluginName + "/releases/latest";
        public string downloadURL => "https://github.com/Squirrelies/" + pluginName + "/releases/download/" + currentVersion + "/" + pluginName + "-v" + currentVersion + ".zip";
        public SRTInfo()
        {
            currentVersion = "0.0.0.0";
        }
    }
}
