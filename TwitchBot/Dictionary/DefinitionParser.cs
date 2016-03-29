using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Dictionary
{
    public class DefinitionParser
    {
        public static void Main(string[] args)
        {
            ImportWords("Dictionary/data.verb", "update Verbs set Definition = @definition where word = @word");
            ImportWords("Dictionary/data.adj", "update Adjectives set Definition = @definition where word = @word");
            ImportWords("Dictionary/data.noun", "update Nouns set Definition = @definition where word = @word");
            ImportWords("Dictionary/data.adv", "update Adverbs set Definition = @definition where word = @word");
        }

        private static void ImportWords(string file, string command)
        {
            string connString = Config.ConnectionString;

            using (StreamReader reader = File.OpenText(file))
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand(command, conn))
            {
                conn.Open();

                var prms = new SqlParameter[] 
                { 
                    new SqlParameter("word", System.Data.SqlDbType.VarChar, 255),
                    new SqlParameter("definition", System.Data.SqlDbType.VarChar, -1)
                };
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

                    int defStart = line.IndexOf('|');
                    if (defStart > 0)
                    {
                        prms[1].Value = line.Substring(defStart + 1).Trim();
                        comm.ExecuteNonQuery();
                    }

                } while (!reader.EndOfStream);

            }
        }

    }
}
