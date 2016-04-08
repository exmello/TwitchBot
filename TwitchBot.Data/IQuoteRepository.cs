using TwitchBot.Model;

namespace TwitchBot.Data
{
    public interface IQuoteRepository
    {
        void AddUpdate(Quote quote);
        Quote GetRandom();
        Quote GetRandomByName(string name);
    }
}
