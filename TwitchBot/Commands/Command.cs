using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Responds to !command or !cmd with a list of valid commands
    /// </summary>
    public class Command : ICommand
    {
        private readonly TwitchResponseWriter tw;
        private readonly Regex regCommand;

        public Command(TwitchResponseWriter tw)
        {
            this.tw = tw;
            this.regCommand = new Regex("^!(command|cmd)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regCommand.IsMatch(message.Content);
        }

        public void Process(MessageInfo message)
        {
            tw.RespondMessage("Commands: !howlong [user], !uptime [channel], !viewers, !madlib [template] (template keywords: noun/adjective/verb/adverb)");
        }
    }
}
