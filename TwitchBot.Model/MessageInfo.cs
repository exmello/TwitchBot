using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Model
{
    public class MessageInfo
    {
        public string Channel { get; set; }
        public MessageActionType Action { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
    }
}
