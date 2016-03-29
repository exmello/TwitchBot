using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Data;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Responds to !blasphemy with a random bible quote with proper nouns replaced by user names
    /// </summary>
    public class Blasphemy : ICommand
    {
        private readonly Regex regBlasphemy;
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;
        private readonly IBlasphemyRepository repo;
        private readonly Random random;

        public Blasphemy(TwitchResponseWriter tw, TwitchApiClient api, IBlasphemyRepository repo)
        {
            this.tw = tw;
            this.api = api;
            this.repo = repo;
            this.regBlasphemy = new Regex("^!blasphemy\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            random = new Random((int)DateTime.Now.Ticks);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regBlasphemy.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {
            //get data
            var data = repo.GetBlasphemy();

            var chatters = api.Chatters(message.Channel);

            if (data != null)
            {
                string response = data.Text;

                response = Regex.Replace(response, "god", message.Channel, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (data.Names.Count() > 0)
                {
                    foreach (string word in data.Names)
                    {
                        if (chatters != null)
                        {
                            string randomViewer = RandomViewer(chatters);

                            response = Regex.Replace(response, word, randomViewer, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                        }
                    }
                }
                else
                {
                    Regex names = new Regex("\\b[A-Z][a-z]+\\b");
                    foreach (Match match in names.Matches(response))
                    {
                        if (chatters != null)
                        {
                            string randomViewer = RandomViewer(chatters);

                            response = Regex.Replace(response, match.Value, randomViewer, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                        }
                    }
                }

                tw.RespondMessage(response);
            }                           
        }

        private string RandomViewer(TwitchApi.Result.ChattersResult chatters)
        {
            string randomViewer = null;
            if ((random.Next(4) == 0 && chatters.chatters.moderators.Length > 0)
                || chatters.chatters.viewers.Length == 0)
            {
                randomViewer = chatters.chatters.moderators[random.Next(chatters.chatters.moderators.Length)];
            }
            else if (chatters.chatters.viewers.Length > 0)
            {
                randomViewer = chatters.chatters.viewers[random.Next(chatters.chatters.viewers.Length)];
            }
            return randomViewer;
        }

    }
}
