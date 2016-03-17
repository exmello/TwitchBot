using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using TwitchBot.TwitchApi.Result;

namespace TwitchBot
{
    public class TwitchApiClient
    {
        public TwitchApiClient()
        {

        }

        private string GetJsonText(string url)
        {
            string jsonText = string.Empty;
           
            using (var client = new System.Net.Http.HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var clientTask = client.SendAsync(request);
                clientTask.Wait();
                HttpResponseMessage response = clientTask.Result;
                response.EnsureSuccessStatusCode();

                var contentTask = response.Content.ReadAsStringAsync();
                contentTask.Wait();
                jsonText = contentTask.Result;
            }

            return jsonText;
        }

        private T ParseJson<T>(string json)
        {
            using (JsonTextReader reader = new JsonTextReader(new StringReader(json)))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public FollowTargetResult FollowTarget(string user, string channel)
        {
            string url = string.Format("http://api.twitch.tv/kraken/users/{0}/follows/channels/{1}", user, channel);
            string json = GetJsonText(url);
            return ParseJson<FollowTargetResult>(json);
        }
    }
}
