using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TwitchBot.Commands;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot
{
    public class KatBot : ITwitchBot
    {
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;
        private readonly string[] ignoreBots;

        public IList<ICommand> CommandList { get; set; }
        public IList<IEvent> EventList { get; set; }
        public IList<IKeyword> KeywordProcessors { get; set; }

        public KatBot(TwitchResponseWriter tw, TwitchApiClient api)
        {
            this.tw = tw;
            this.api = api;
            this.ignoreBots = new string[] { "moobot", "nightbot", "whale_bot" };
            
            CommandList = new List<ICommand>();
            EventList = new List<IEvent>();
            KeywordProcessors = new List<IKeyword>();
            
            // Lets you know its working
            //tw.RespondMessage("MrDestructoid MrDestructoid MrDestructoid MrDestructoid MrDestructoid MrDestructoid MrDestructoid");
            //tw.RespondMessage("test");
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
            else
            {
                RespondToEvents(message);
            }
        }

        public void ProcessChatMessage(MessageInfo message)
        {
            if (!(Config.IgnoreSelf && message.Username.Equals(Config.Nickname, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (message.Content.StartsWith("!"))
                    RespondToCommands(message);

                RespondToKeywords(message);
            }
        }

        private void RespondToCommands(MessageInfo message)
        {
            foreach (ICommand command in CommandList.OfType<ICommand>())
            {
                if(command.IsMatch(message))
                {
                    command.Process(message);
                    return;
                }
            }
        }

        private void RespondToEvents(MessageInfo message)
        {
            foreach (IEvent evnt in CommandList.OfType<IEvent>())
            {
                evnt.Process(message);
            }
        }

        private void RespondToKeywords(MessageInfo message)
        {
            foreach (IKeyword keyword in KeywordProcessors)
            {
                keyword.Process(message);
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

        public void Update()
        {
            //implement
        }
    }
}
