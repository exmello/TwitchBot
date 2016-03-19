using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TwitchBot.Commands;
using TwitchBot.Model;

namespace TwitchBot
{
    public class KatBot : ITwitchBot
    {
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;
        private readonly string[] ignoreBots;

        public IList<ICommand> CommandList { get; set; }

        public KatBot(TwitchResponseWriter tw, TwitchApiClient api)
        {
            this.tw = tw;
            this.api = api;
            this.ignoreBots = new string[] { "moobot", "nightbot", "whale_bot" };
            
            CommandList = new List<ICommand>();
            
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
                //ProcessJoinEvent(message.Username);
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

        private void RespondToCommands(MessageInfo message)
        {
            foreach (ICommand command in CommandList)
            {
                if(command.IsMatch(message))
                {
                    command.Process(message);
                    return;
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
