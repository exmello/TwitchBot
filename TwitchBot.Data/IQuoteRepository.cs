using System.Collections.Generic;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public interface IQuoteRepository
    {
        void AddUpdate(Quote quote);
        Quote GetRandom();
        Quote GetRandomByName(string name);
        IEnumerable<Quote> GetAll(string channelName);
        void Admin_AddUpdate(Quote quote, string channelName);
        Quote GetByID(int id, string channelName);
        void Delete(int id, string channelName);
    }
}
