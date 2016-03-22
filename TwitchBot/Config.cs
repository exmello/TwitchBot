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

        private static string _chatServer = null;
        public static string ChatServer
        {
            get
            {
                if (_chatServer == null)
                    _chatServer = ConfigurationManager.AppSettings["TwitchBot.ChatServer"];
                return _chatServer;
            }
        }

        private static int? _port;
        public static int Port
        {
            get
            {
                if (!_port.HasValue)
                    _port = int.Parse(ConfigurationManager.AppSettings["TwitchBot.Port"]);
                return _port.Value;
            }
        }

        private static bool? _ignoreSelf;
        public static bool IgnoreSelf
        {
            get
            {
                if (!_ignoreSelf.HasValue)
                    _ignoreSelf = bool.Parse(ConfigurationManager.AppSettings["TwitchBot.IgnoreSelf"]);
                return _ignoreSelf.Value;
            }
        }

        private static string _connectionString = null;
        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                    _connectionString = ConfigurationManager.AppSettings["TwitchBot.ConnectionString"];
                return _connectionString;
            }
        }
    }
}
