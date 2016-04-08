using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public class SqlChannelRepository : IChannelRepository
    {
        public IEnumerable<Keyword> GetAllKeywords()
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Keyword_GetAll", conn))
            {
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    yield return new Keyword
                    {
                        ID = (int)reader["ID"],
                        Regex = (string)reader["Regex"],
                        Message = (string)reader["Message"],
                        Username = reader["Username"] != DBNull.Value ? (string)reader["Username"] : null
                    };
                }
            }
        }

        public IEnumerable<Nickname> GetAllNicknames()
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Nickname_GetAll", conn))
            {
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    yield return new Nickname
                    {
                        Username = (string)reader["Username"],
                        Regex = (string)reader["Regex"]
                    };
                }
            }
        }


        public ChannelForDashboardResult GetChannelForDashboard(string channelName)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Channel_GetForDashboard @ChannelName", conn))
            {
                conn.Open();

                var prms = new SqlParameter[] { new SqlParameter("ChannelName", System.Data.SqlDbType.VarChar, 255) { Value = channelName } };

                comm.Parameters.AddRange(prms);

                SqlDataReader reader = comm.ExecuteReader();

                ChannelForDashboardResult result = null;
                if (reader.Read())
                {
                    result = new ChannelForDashboardResult
                    {
                        Name = (string)reader["Name"],
                        StreamCount = (int)reader["StreamCount"],
                        KeywordCount = (int)reader["KeywordCount"],
                        QuoteCount = (int)reader["QuoteCount"],
                        QuoteIncreasePercent = (decimal)reader["QuoteIncreasePercent"]
                    };
                }

                return result;
            }
        }
    }
}
