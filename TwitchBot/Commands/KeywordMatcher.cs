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
    /// Responds to keywords based on regex matches to settings data
    /// </summary>
    public class KeywordMatcher : IKeyword
    {
        private readonly TwitchResponseWriter tw;
        private readonly ISettingsRepository repo;

        private IList<Regex> regKeywords = null;
        private IList<Keyword> keywords = null;

        public KeywordMatcher(TwitchResponseWriter tw, ISettingsRepository repo)
        {
            this.tw = tw;
            this.repo = repo;
        }

        public bool IsMatch(MessageInfo message)
        {
            return false;
        }

        void IKeyword.Process(MessageInfo message)
        {
            if(regKeywords == null)
            {
                LoadKeywords();
            }

            string response = string.Empty;

            for (int i = 0; i < regKeywords.Count; i++)
			{
                if(regKeywords[i].IsMatch(message.Content))
                {
                    response = keywords[i].Message;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(response))
            {
                tw.RespondMessage(response);
            }
            
        }

        private void LoadKeywords()
        {
            var keywordData = repo.GetAllKeywords();

            if (keywordData != null)
            {
                regKeywords = keywordData.Select(k => new Regex(k.Regex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)).ToList();
                keywords = keywordData.ToList();
            }
        }


    }
}
