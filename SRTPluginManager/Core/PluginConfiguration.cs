namespace SRTPluginManager.Core
{
    public class PluginConfiguration
    {
        public PluginInfo[] PluginConfig { get; set; }
        public PluginInfo SRTConfig { get; set; }
        public PluginInfo[] ExtensionsConfig { get; set; }

        public PluginConfiguration()
        {
            PluginConfig = new PluginInfo[11];
            for (var i = 0; i < PluginConfig.Length; i++)
            {
                PluginConfig[i] = new PluginInfo();
            }

            SRTConfig = new PluginInfo();
            SRTConfig.pluginName = "SRTHost";
            SRTConfig.currentVersion = "0.0.0.0";

            ExtensionsConfig = new PluginInfo[2];
            for (var i = 0; i < ExtensionsConfig.Length; i++)
            {
                ExtensionsConfig[i] = new PluginInfo();
            }
        }
    }
}
