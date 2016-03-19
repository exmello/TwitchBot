using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Tests if an input message matches the command
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool IsMatch(MessageInfo message);

        /// <summary>
        /// Processes input
        /// </summary>
        /// <param name="input"></param>
        void Process(MessageInfo message);
    }
}
