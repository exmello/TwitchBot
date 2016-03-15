using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitchBot
{
    public class KatBot
    {
        private readonly TwitchResponseWriter tw;
        public KatBot(TwitchResponseWriter tw)
        {
            this.tw = tw;

            // Lets you know its working
            tw.RespondMessage("BEEP BOOP");
            Console.WriteLine("BEEP BOOP CHAT STARTING.\r\n\r\n.");
        }

        public void ProcessMessage(string username, string message)
        {
            if (!(username.Equals(Config.Nickname, StringComparison.InvariantCultureIgnoreCase) && message.StartsWith(Config.BotName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (message.Contains("lettuce"))
                {
                    tw.RespondMessage("yummy");
                }
            }
        }

        public void ProcessJoinEvent(string username)
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
