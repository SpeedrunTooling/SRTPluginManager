namespace SRTPluginManager.Core
{
    public class PluginInfo
    {
        public int platform { get; set; }
        public string internalName { get; set; }
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string downloadURL { get; set; }
        public string[] contributors { get; set; }
        public PluginInfo()
        {
        }
    }
}
