using Newtonsoft.Json;
using System.IO;
using System.Net.Http;

namespace TwitchBot.Akinator.Api
{
    public class AkinatorApiClient
    {
        private string _url = "http://api-usa4.akinator.com/ws/";

        public AkinatorApiClient()
        {

        }

        private string GetJsonText(string url)
        {
            string jsonText = string.Empty;
           
            using (var client = new System.Net.Http.HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                var clientTask = client.SendAsync(request);
                clientTask.Wait();
                HttpResponseMessage response = clientTask.Result;

                if (!response.IsSuccessStatusCode)
                    return null;
                //response.EnsureSuccessStatusCode();

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

        public NewSessionResponse NewSession(string user)
        {
            string url = string.Format("{0}new_session?partner=1&player={1}", _url, user);
            string json = GetJsonText(url);

            if (json == null)
                return null;

            return ParseJson<NewSessionResponse>(json);
        }

        public AnswerResponse Answer(string session, string signature, int step, int answerID)
        {
            string url = string.Format("{0}answer?session={1}&signature={2}&step={3}&answer={4}", _url, session, signature, step, answerID);
            string json = GetJsonText(url);

            if (json == null)
                return null;

            return ParseJson<AnswerResponse>(json);
        }

        public ListResponse List(string session, string signature, int step)
        {
            string url = string.Format("{0}list?session={1}&signature={2}&step={3}&size=2&max_pic_width=246&max_pic_height=294&pref_photos=VO-OK&mode_question=0", _url, session, signature, step);
            string json = GetJsonText(url);

            if (json == null)
                return null;

            return ParseJson<ListResponse>(json);
        }
    }
}
