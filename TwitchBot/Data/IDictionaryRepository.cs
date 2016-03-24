using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Dictionary;

namespace TwitchBot.Data
{
    public interface IDictionaryRepository
    {
        WordsResult GetRandomWords();
    }
}
