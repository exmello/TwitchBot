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
    public class SqlBlasphemyRepository : IBlasphemyRepository
    {
        public VerseNames GetBlasphemy()
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Verses_GetBlasphemy", conn))
            {
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                if (reader.Read())
                {
                    string text = (string)reader["Text"];

                    reader.NextResult();

                    var names = new List<string>();

                    while(reader.Read())
                    {
                        names.Add((string)reader["Word"]);
                    }

                    return new VerseNames
                    {
                        Text = text,
                        Names = names
                    };
                }

                return null;
            }
        }

    }
}
