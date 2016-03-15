using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchBot
{
    public class KatBot
    {
        private readonly TwitchResponseWriter tw;

        private Regex regLettuce = new Regex("lettuce", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private Regex regKebab = new Regex("kebab|kabob", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private Regex regNips = new Regex("nips", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        
        public KatBot(TwitchResponseWriter tw)
        {
            this.tw = tw;

            // Lets you know its working
            //tw.RespondMessage(": Activating MrDestructoid");
            //Console.WriteLine("BEEP BOOP CHAT STARTING.\r\n\r\n.");
        }

        public void ProcessMessage(string username, string message)
        {
            if (!(username.Equals(Config.Nickname, StringComparison.InvariantCultureIgnoreCase) && message.StartsWith(Config.BotName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (regNips.IsMatch(message))
                {
                    tw.RespondMessage(": It's KatNIP. No 's'! BibleThump");
                }
                else if (regLettuce.IsMatch(message))
                {
                    tw.RespondMessage(": Did someone say LETTUCE? PogChamp");
                }
                else if (regKebab.IsMatch(message))
                {
                    tw.RespondMessage(": KEBAB? I prefer lettuce. FailFish");
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
