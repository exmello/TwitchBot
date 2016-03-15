using System.Configuration;

namespace TwitchBot
{
    public static class Config
    {
        private static string _channelName = null;
        public static string ChannelName 
        { 
            get
            {
                if (_channelName == null)
                    _channelName = ConfigurationManager.AppSettings["TwitchBot.ChannelName"];
                return _channelName;
            }
        }

        //Convert your password to OAuth using this link: https://twitchapps.com/tmi/
        private static string _oath = null;
        public static string OAuth
        {
            get
            {
                if (_oath == null)
                    _oath = ConfigurationManager.AppSettings["TwitchBot.OAuth"];
                return _oath;
            }
        }

        private static string _nickname = null;
        public static string Nickname
        {
            get
            {
                if (_nickname == null)
                    _nickname = ConfigurationManager.AppSettings["TwitchBot.Nickame"];
                return _nickname;
            }
        }

        private static string _botname = null;
        public static string BotName
        {
            get
            {
                if (_botname == null)
                    _botname = ConfigurationManager.AppSettings["TwitchBot.BotName"];
                return _botname;
            }
        }
    }
}
