using System.Data.SqlClient;
using System.IO;

namespace TwitchBot.Data.Dictionary
{
    public class DictionaryParser
    {
        public static void Main(string[] args)
        {
            ImportWords("Dictionary/data.verb", "if not exists(select 1 from Verbs where word = @word) insert into Verbs values (@word)");
            ImportWords("Dictionary/data.adj", "if not exists(select 1 from Adjectives where word = @word) insert into Adjectives values (@word)");
            ImportWords("Dictionary/data.noun", "if not exists(select 1 from Nouns where word = @word) insert into Nouns values (@word)");
            ImportWords("Dictionary/data.adv", "if not exists(select 1 from Adverbs where word = @word) insert into Adverbs values (@word)");
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
