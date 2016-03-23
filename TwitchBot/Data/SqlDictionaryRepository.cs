using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Dictionary;

namespace TwitchBot.Data
{
    public class SqlDictionaryRepository : IDictionaryRepository
    {
        public WordsResult GetRandomWords()
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Dictionary_GetRandomWords", conn))
            {
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                if (reader.Read())
                {
                    return new WordsResult
                    {
                        Noun = (string)reader["Noun"],
                        Verb = (string)reader["Verb"],
                        Adverb = (string)reader["Adverb"],
                        Adjective = (string)reader["Adjective"]
                    };
                }

                return null;
            }
        }

    }
}
