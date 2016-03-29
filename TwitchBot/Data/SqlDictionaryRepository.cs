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

        public DefinitionResult GetDefinition(string word)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Dictionary_GetDefinition @Word", conn))
            {
                conn.Open();

                var prms = new SqlParameter[] { new SqlParameter("Word", System.Data.SqlDbType.VarChar, -1) { Value = word } };

                comm.Parameters.AddRange(prms);

                SqlDataReader reader = comm.ExecuteReader();

                if (reader.Read())
                {
                    return new DefinitionResult
                    {
                        Word = (string)reader["Word"],
                        Definition = (string)reader["Definition"],
                        PartOfSpeech = (string)reader["PartOfSpeech"],
                    };
                }

                return null;
            }
        }

    }
}
