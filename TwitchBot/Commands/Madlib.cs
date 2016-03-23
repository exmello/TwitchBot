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
    public class Madlib : ICommand
    {
        private readonly Regex regMadlib;
        private readonly TwitchResponseWriter tw;
        private readonly IDictionaryRepository repo;

        public Madlib(TwitchResponseWriter tw, IDictionaryRepository repo)
        {
            this.tw = tw;
            this.repo = repo;
            this.regMadlib = new Regex("^!madlib\\s(?<template>.*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regMadlib.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {
            //get template
            string template = string.Empty;
            Match match = regMadlib.Match(message.Content);
            if (match.Success)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["template"].Value))
                {
                    if (match.Groups["template"].Value.Contains(' '))
                    {
                        template = match.Groups["template"].Value;
                    }
                    else
                    {
                        //assume user
                        template = string.Format("{0} can you please not adverb verb a adjective noun?", match.Groups["template"].Value);        
                    }
                }
                else
                {
                    template = string.Format("I want {0} to verb my noun.", message.Username);                    
                }
            }

            //get data
            var randomWords = repo.GetRandomWords();

            if (randomWords != null)
            {
                string response = template
                    .Replace("adverb", randomWords.Adverb)
                    .Replace("verb", randomWords.Verb)
                    .Replace("adjective", randomWords.Adjective)
                    .Replace("noun", randomWords.Noun);

                tw.RespondMessage(response);
            }
        }

    }
}
