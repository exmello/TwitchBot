using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class Quote : ICommand, IKeyword
    {
        private readonly TwitchResponseWriter tw;
        private readonly IChannelRepository settingRepo;
        private readonly IQuoteRepository quoteRepo;

        private const string DATE_FORMAT = "MMM yyyy";

        private IList<NicknameRegex> nicknameRegex = null;
        private Regex commandRegex = new Regex("^!quote\\s?@?(?<attribution>[0-9a-zA-Z_]*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private Regex quoteRegex = new Regex("^\"(?<text>-?[^\"]+?)\"[- ]*@?(?<attribution>[0-9a-zA-Z_]*?)\\s.*\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public Quote(TwitchResponseWriter tw, IChannelRepository settingRepo, IQuoteRepository quoteRepo)
        {
            this.tw = tw;
            this.settingRepo = settingRepo;
            this.quoteRepo = quoteRepo;

            LoadNicknames();
        }

        public bool IsMatch(MessageInfo message)
        {
            return commandRegex.IsMatch(message.Content);
        }

        void IKeyword.Process(MessageInfo message)
        {
            string response = string.Empty;

            Match match = quoteRegex.Match(message.Content);
            if (match.Success)
            {
                string attribution = match.Groups["attribution"].Value;
                string quoteText = match.Groups["text"].Value;
                DateTime attributionDate = DateTime.Now;

                //match nicknames
                NicknameRegex nickMatch = nicknameRegex.FirstOrDefault(n => n.Regex.IsMatch(attribution));
                if (nickMatch != null)
                    attribution = nickMatch.Nickname.Username;

                Model.Quote quote = new Model.Quote
                {
                    Text = quoteText,
                    AttributedTo = attribution,
                    AttributedDate = DateTime.Now, //TODO: parse this from regex
                    CreatedBy = message.Username
                };

                //add to db
                quoteRepo.AddUpdate(quote);

                tw.RespondMessage(string.Format("Added new quote: \"{0}\" -{1} {2}"
                    , quoteText
                    , attribution
                    , attributionDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)));
            }
        }

        void ICommand.Process(MessageInfo message)
        {
            Match match = commandRegex.Match(message.Content);
            if (match.Success)
            {
                Model.Quote quote;
                if (!string.IsNullOrWhiteSpace(match.Groups["attribution"].Value))
                {
                    string attribution = match.Groups["attribution"].Value;

                    //match nicknames
                    NicknameRegex nickMatch = nicknameRegex.FirstOrDefault(n => n.Regex.IsMatch(attribution));
                    if (nickMatch != null)
                        attribution = nickMatch.Nickname.Username;

                    quote = quoteRepo.GetRandomByName(attribution);
                }
                else
                {
                    quote = quoteRepo.GetRandom();
                }

                if (quote != null)
                {
                    tw.RespondMessage(string.Format("\"{0}\" -{1} {2}"
                    , quote.Text
                    , quote.AttributedTo
                    , quote.AttributedDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)));
                }
            }
        }

        private void LoadNicknames()
        {
            var nicknames = settingRepo.GetAllNicknames();

            if (nicknames != null)
            {
                nicknameRegex = nicknames.Select(n => new NicknameRegex
                {
                    Nickname = n,
                    Regex = new Regex(n.Regex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                }).ToList();
            }
        }

        internal class NicknameRegex
        {
            public Nickname Nickname { get; set; }
            public Regex Regex { get; set; }
        }
    }
}
