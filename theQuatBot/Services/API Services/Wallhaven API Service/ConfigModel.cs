using System.Collections.Generic;

namespace TheQuatBot.Services.API_Services.Wallhaven_API_Service
{
    public class ConfigModel
    {
        private Dictionary<string, string> rawData = new Dictionary<string, string>();
        public string API_KEY;
    }
}
