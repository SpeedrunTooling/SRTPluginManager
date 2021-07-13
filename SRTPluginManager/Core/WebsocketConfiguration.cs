namespace SRTPluginManager.Core
{
    public class WebsocketConfiguration
    {
        public string Token { get; set; }
        public string Key { get; set; }

        public bool LowBandwithMode { get; set; }

        public WebsocketConfiguration()
        {
            Token = "";
            Key = "";
            LowBandwithMode = true;
        }
    }
}
