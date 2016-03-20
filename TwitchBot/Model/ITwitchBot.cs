
namespace TwitchBot.Model
{
    interface ITwitchBot
    {
        void ProcessMessage(MessageInfo message);
        void Update();
    }
}
