using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot.Commands
{
    public interface IEvent
    {
        /// <summary>
        /// Processes an event
        /// </summary>
        /// <param name="input"></param>
        void Process(MessageInfo message);
    }
}
