using System.Configuration;

namespace TwitchBot.Data
{
    public static class Config
    {
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
