using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Model
{
    public enum MessageActionType
    {
        Unknown,
        Join, // "JOIN"
        Message // "PRIVMSG"
    }
}
