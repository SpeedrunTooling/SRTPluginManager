namespace SRTPluginManager.Core
{
    public class PluginConfiguration
    {
        public PluginInfo ManagerConfig { get; set; }
        public PluginInfo[] PluginConfig { get; set; }
        public PluginInfo SRTConfig { get; set; }
        public PluginInfo[] ExtensionsConfig { get; set; }
        public PluginInfo[] InterfaceConfig { get; set; }
        public PluginConfiguration()
        {
        }
    }
}
