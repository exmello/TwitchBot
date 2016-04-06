using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot.Commands
{
    /// <summary>
    /// When a short URL is detected, respond with the expanded URL
    /// Idea from: https://github.com/aadityabhatia/url-expander/blob/master/app.py
    /// </summary>
    public class UrlExpander : IKeyword
    {
        private readonly TwitchResponseWriter tw;
        private readonly Regex regUrl;
        private readonly MemoryCache cache;

        private const int cacheDurationTempRedirect = 86400; //1 day

        public UrlExpander(TwitchResponseWriter tw)
        {
            this.tw = tw;
            this.regUrl = new Regex("(https?:\\/\\/[a-zA-Z0-9\\._\\/-]{5,25})\\s", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            this.cache = new MemoryCache();
        }

        public bool IsMatch(MessageInfo message)
        {
            return regUrl.IsMatch(message.Content);
        }

        public void Process(MessageInfo message)
        {
            Match match = regUrl.Match(message.Content);

            if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[0].Value))
            {
                string shortUrl = match.Groups[0].Value.Trim();

                string fullUrl = ExpandUrl(shortUrl);

                if(!string.IsNullOrEmpty(fullUrl))
                {
                    tw.RespondMessage(string.Format("{0} redirects to {1}", shortUrl, fullUrl));
                }
            }
        }

        private string ExpandUrl(string shortUrl)
        {
            CachedUrl cachedUrl = cache.Get(shortUrl);
            string fullUrl;

            if(cachedUrl != null)
            {
                return cachedUrl.FullUrl;
            }

            var handler = new HttpClientHandler() { AllowAutoRedirect = false };

            using (var client = new System.Net.Http.HttpClient(handler))
            using (var request = new HttpRequestMessage(HttpMethod.Head, shortUrl))
            {
                
                var clientTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                clientTask.Wait();
                HttpResponseMessage response = clientTask.Result;

                if (response.StatusCode == HttpStatusCode.MethodNotAllowed) //405
                {
                    clientTask = client.GetAsync(shortUrl, HttpCompletionOption.ResponseHeadersRead);
                    clientTask.Wait();
                }

                
                HttpStatusCode code = response.StatusCode;

                if ((int)code == 301 || (int)code == 302 || (int)code == 303 || (int)code == 307)
                {
                    fullUrl = response.Headers.Location.ToString();
                }
                else
                {
                    return null;
                }

                DateTime expires = ((int)code == 301) ? DateTime.MaxValue : DateTime.Now.AddSeconds(cacheDurationTempRedirect);

                cache.Set(new CachedUrl { ShortUrl = shortUrl, FullUrl = fullUrl, Expires = expires });

                return fullUrl;
            }
        }

        internal class CachedUrl
        {
            public string ShortUrl { get; set; }
            public string FullUrl { get; set; }
            public DateTime Expires { get; set; }
        }

        internal class MemoryCache
        {
            public Dictionary<string, CachedUrl> Items { get; set; }

            public MemoryCache()
            {
                Items = new Dictionary<string, CachedUrl>();
            }

            public void Set(CachedUrl value)
            {
                if(Items.ContainsKey(value.ShortUrl))
                {
                    Items[value.ShortUrl] = value;
                }
                else
                {
                    Items.Add(value.ShortUrl, value);
                }

                List<string> removedItems = null;
                foreach (var item in Items)
                {
                    if(item.Value.Expires < DateTime.Now)
                    {
                        if (removedItems == null)
                            removedItems = new List<string>();
                        removedItems.Add(item.Key);
                    }
                }
                if (removedItems != null)
                {
                    foreach (string key in removedItems)
                    {
                        Items.Remove(key);
                    }
                }
            }

            public CachedUrl Get(string shortUrl)
            {
                if(Items.ContainsKey(shortUrl))
                {
                    if (Items[shortUrl].Expires > DateTime.Now)
                        return Items[shortUrl];
                    else
                        Items.Remove(shortUrl);
                }

                return null;
            }
        }
    }
}
