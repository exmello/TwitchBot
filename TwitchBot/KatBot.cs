using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TwitchBot.Model;

namespace TwitchBot
{
    public class KatBot : ITwitchBot
    {
        private readonly TwitchResponseWriter tw;
        private readonly string[] ignoreBots;
        private readonly TwitchApiClient api;
        
        public KatBot(TwitchResponseWriter tw)
        {
            this.tw = tw;
            this.ignoreBots = new string[] { "moobot", "nightbot", "whale_bot" };
            api = new TwitchApiClient();
            // Lets you know its working
            //tw.RespondMessage("KatBot Activating MrDestructoid");
        }

        public void ProcessMessage(MessageInfo message)
        {
            if (message.Action == MessageActionType.Message)
            {
                // Ignore some well known bots
                if (!ignoreBots.Contains(message.Username.ToLowerInvariant()))
                {
                    //Act on message content
                    ProcessChatMessage(message);
                }
            }   
            else if (message.Action == MessageActionType.Join)
            {
                //Act on join event
                ProcessJoinEvent(message.Username);
            }
        }

        public void ProcessChatMessage(MessageInfo message)
        {
            if (!(Config.IgnoreSelf && message.Username.Equals(Config.Nickname, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (message.Content.StartsWith("!"))
                    RespondToCommands(message);

                //RespondToKeywords(message);
            }
        }

        private Regex regHowLong = new Regex("^!howlong\\s(?<user>[0-9a-zA-Z_]*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private Regex regCommand = new Regex("^!(command|cmd)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private void RespondToCommands(MessageInfo message)
        {
            if (regCommand.IsMatch(message.Content))
            {
                tw.RespondMessage("Commands: !howlong, !howlong <user>");
                return;
            }

            Match match = regHowLong.Match(message.Content);
            if (match.Success)
            {
                if (match.Groups.Count > 0)
                {
                    if (match.Groups["user"].Value.ToLowerInvariant() == "roflgator")
                        tw.RespondMessage("16 inches");
                    else
                        HowLong(match.Groups["user"].Value, Config.ChannelName);
                }
                else
                    HowLong(message.Username, Config.ChannelName);
                return;
            }
        }

        private void HowLong(string username, string channel)
        {
            var followData = api.FollowTarget(username, channel);

            if (followData != null)
            {
                int months = ((DateTime.Now.Year - followData.created_at.Year) * 12) + DateTime.Now.Month - followData.created_at.Month;
                if (months > 0)
                {
                    tw.RespondMessage(string.Format("user @{0} has been following {1} for {2} month{3}!"
                        , username, channel, months, months == 1 ? string.Empty : "s"));
                }
                else
                {
                    int days = Convert.ToInt32((DateTime.Now - followData.created_at).TotalDays);
                    tw.RespondMessage(string.Format("user @{0} has been following {1} for {2} day{3}!"
                        , username, channel, days, days == 1 ? string.Empty : "s"));
                }
            }
        }

        private Regex regLettuce = new Regex("lettuce", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private Regex regKebab = new Regex("kebab|kabob", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private Regex regNips = new Regex("nips", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private void RespondToKeywords(MessageInfo message)
        {
            if (regNips.IsMatch(message.Content))
            {
                tw.RespondMessage("It's KatNIP. No 's'! BibleThump");
            }
            else if (regLettuce.IsMatch(message.Content))
            {
                tw.RespondMessage("Did someone say LETTUCE? PogChamp");
            }
            else if (regKebab.IsMatch(message.Content))
            {
                tw.RespondMessage("KEBAB? I prefer lettuce. FailFish");
            }
        }

        private void ProcessJoinEvent(string username)
        {
            tw.RespondMessage(string.Format("Welcome {0}!", username));
        }

        private void BanUser(string username)
        {
            //implement
        }

        private void TimeoutUser(string username, int timeout)
        {
            //implement
        }
    }
}
