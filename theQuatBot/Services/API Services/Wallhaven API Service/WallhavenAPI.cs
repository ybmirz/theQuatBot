using System.Net;

namespace TheQuatBot.Services
{
    public class WallhavenAPI
    {
        private string baseEndpoint = "https://wallhaven.cc/api/v1/";
        private string SearchTagEndpoint = "search?&q=";

        public WallhavenAPI()
        {
            GetConfiguration();
        }

        private void GetConfiguration()
        {
            //load config if you need to
        }

        public WallpaperModel searchTag(string tag)
        {
            string endpointToCall = baseEndpoint + SearchTagEndpoint + tag;
            endpointToCall = AuthenticateRequest(endpointToCall);
            endpointToCall = SetPurityLevel(endpointToCall);
            WallpaperModel pM = WallpaperModel.FromJson(GetJsonFromEndpoint(endpointToCall));
            return pM;
        }

        public WallpaperModel searchTagWithPage(string tag, int page)
        {
            SearchTagEndpoint = $"search?&page={page}&q="; 
            string endpointToCall = baseEndpoint + SearchTagEndpoint + tag;
            endpointToCall = AuthenticateRequest(endpointToCall);
            endpointToCall = SetPurityLevel(endpointToCall);
            WallpaperModel pM = WallpaperModel.FromJson(GetJsonFromEndpoint(endpointToCall));
            return pM;
        }

        private string AuthenticateRequest(string unauthenticatedRequest)
        {
            string authenticated = unauthenticatedRequest + "&apikey=";  //+ loadedConfig.API_KEY;
            return authenticated;
        }

        private string SetPurityLevel(string request)
        {
            string result = request + "&purity="; //loadedConfig.GetPurityString();
            return result;
        }

        private string GetJsonFromEndpoint(string url)
        {
            string jsonResult = "";
            using (WebClient wc = new WebClient())
            {
                jsonResult = wc.DownloadString(url);
            }
            return jsonResult;
        }
    }
  }


