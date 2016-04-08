
namespace TwitchBot.Model
{
    public interface ITwitchBot
    {
        void ProcessMessage(MessageInfo message);
        void Update();
    }
}
