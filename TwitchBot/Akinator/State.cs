using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Akinator
{
    public enum State
    {
        Disabled,
        Stopped,
        Started,
        ListeningForAnswers,
        ListeningForYesNo
    }
}
