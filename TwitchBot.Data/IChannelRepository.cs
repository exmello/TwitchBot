using System.Collections.Generic;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public interface IChannelRepository
    {
        IEnumerable<Keyword> GetAllKeywords();
        IEnumerable<Nickname> GetAllNicknames();

        ChannelForDashboardResult GetChannelForDashboard(string channelName);
    }
}
