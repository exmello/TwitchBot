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
    /// Responds to !define with a dictionary definition.  balderdash game potential?
    /// </summary>
    public class Define : ICommand
    {
        private readonly Regex regDefine;
        private readonly TwitchResponseWriter tw;
        private readonly IDictionaryRepository repo;

        public Define(TwitchResponseWriter tw, IDictionaryRepository repo)
        {
            this.tw = tw;
            this.repo = repo;
            this.regDefine = new Regex("^!define\\s(?<word>.*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regDefine.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {
            //get template
            string template = string.Empty;
            Match match = regDefine.Match(message.Content);
            if (match.Success && !string.IsNullOrWhiteSpace(match.Groups["word"].Value))
            {
                //get data
                var definition = repo.GetDefinition(match.Groups["word"].Value);

                if (definition != null)
                {
                    tw.RespondMessage(string.Format("{0} ({1}): {2}", definition.Word, definition.PartOfSpeech.ToLowerInvariant(), definition.Definition));
                }   
            }
                        
        }

    }
}
