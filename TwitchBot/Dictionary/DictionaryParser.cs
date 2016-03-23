using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Dictionary
{
    public class DictionaryParser
    {
        public static void Main(string[] args)
        {
            //ImportWords("Dictionary/data.verb", "insert into Verbs_import values (@word)");
            //ImportWords("Dictionary/data.adj", "insert into Adjectives_import values (@word)");
            //ImportWords("Dictionary/data.noun", "insert into Nouns_import values (@word)");
            //ImportWords("Dictionary/data.adv", "insert into Adverbs_import values (@word)");
        }

        private static void ImportWords(string file, string command)
        {
            string connString = Config.ConnectionString;

            using (StreamReader reader = File.OpenText(file))
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand(command, conn))
            {
                conn.Open();

                var prms = new SqlParameter[] { new SqlParameter("word", System.Data.SqlDbType.VarChar, 255) };
                comm.Parameters.AddRange(prms);

                int start = 17;

                do
                {
                    string line = reader.ReadLine();

                    if (line[0] == ' ')
                        continue;

                    int end = start;

                    for (int i = start; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            end = i - start;
                            break;
                        }
                    }

                    prms[0].Value = line.Substring(start, end).Replace("_"," ");
                    comm.ExecuteNonQuery();

                } while (!reader.EndOfStream);

            }
        }

    }
}
