using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Dictionary;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public class SqlSettingsRepository : ISettingsRepository
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

    }
}
