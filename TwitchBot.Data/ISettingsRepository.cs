using System.Collections.Generic;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public interface ISettingsRepository
    {
        IEnumerable<Keyword> GetAllKeywords();
        IEnumerable<Nickname> GetAllNicknames();
    }
}
