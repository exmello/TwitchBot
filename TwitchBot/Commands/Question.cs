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
    /// Responds to !viewer with how many viewers have watched the current stream from beginning to end
    /// </summary>
    public class Question : IKeyword
    {
        private readonly Regex regQuestion;
        private readonly Regex regPretty;
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;
        private readonly IDictionaryRepository repo;
        private readonly Random random;

        public Question(TwitchResponseWriter tw, TwitchApiClient api, IDictionaryRepository repo)
        {
            this.tw = tw;
            this.api = api;
            this.repo = repo;
            this.regQuestion = new Regex("^@(?<user>[a-zA-Z_0-9]+?)\\s+(?<prefix>(who|which|where|when|what|why|can|do|is|does|are|how|will)?)(\\s.*)?[?]\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            this.regPretty = new Regex("pretty|beautiful|wonderful|sexy|bae|(best lucio)|(best mercy)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            random = new Random((int)DateTime.Now.Ticks);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regQuestion.IsMatch(message.Content);
        }

        void IKeyword.Process(MessageInfo message)
        {
            //get template
            string template = string.Empty;
            Match match = regQuestion.Match(message.Content);
            if (match.Success 
                && !string.IsNullOrWhiteSpace(match.Groups["user"].Value) && match.Groups["user"].Value.ToLowerInvariant() == Config.Nickname
                && !string.IsNullOrWhiteSpace(match.Groups["prefix"].Value))
            {
                string subject = match.Groups["user"].Value;
                string answer = string.Empty;

                switch (match.Groups["prefix"].Value.ToLowerInvariant())
                {
                    case "who":
                    case "which":
                        answer = Who(message);
                        break;
                    case "where":
                    case "how":
                        answer = What();
                        break;
                    case "when":
                        answer = "Soon.";
                        break;
                    case "what":
                        answer = What();
                        break;
                    case "why":
                        answer = What() + " and why not?";
                        break;
                    default:
                        answer = Can();
                        break;
                }

                if (!string.IsNullOrEmpty(answer))
                {
                    tw.RespondMessage(answer);
                }
            }
        }

        private string Can()
        {
            string[] phrases = { "yes", "no", "maybe", "probably", "probably not", "idk ask ur mom" };
            return phrases[random.Next(phrases.Length)];
        }

        private string What()
        {
            var randomWords = repo.GetRandomWords();

            if (random.Next(6) == 1)
                return "Yes.";
            return string.Format("{0} {1}", randomWords.Adjective, randomWords.Noun);
        }

        private string Who(MessageInfo message)
        {
            if (regPretty.IsMatch(message.Content))
                return message.Channel;

            var chatters = api.Chatters(message.Channel);
            if (chatters != null && chatters.chatters.viewers.Length > 0)
                    return chatters.chatters.viewers[random.Next(chatters.chatters.viewers.Length)];

            string[] phrases = { "ur mom", "Who knows?", "You know the answer...", "I'm not touching that.", "@itskatnipbot" };
            return phrases[random.Next(phrases.Length)];
        }

    }
}
