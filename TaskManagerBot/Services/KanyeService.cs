using Newtonsoft.Json.Linq;
using RestSharp;

namespace TheQuatBot.Services
{
    public class KanyeService
    {
        public static string GetQuote()
        {
            var client = new RestClient("https://api.kanye.rest/");
            var request = new RestRequest("format?=json", Method.GET, DataFormat.Json);
            var response = client.Execute(request);

            //parse json response
            var jObj = JObject.Parse(response.Content);

            return jObj.GetValue("quote").ToString();
        }

    }
}
