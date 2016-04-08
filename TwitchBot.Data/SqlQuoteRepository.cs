using System;
using System.Data.SqlClient;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public class SqlQuoteRepository : IQuoteRepository
    {
        public void AddUpdate(Quote quote)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Quote_AddUpdate @Text, @AttributedTo, @AttributedDate, @CreatedBy", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("Text", System.Data.SqlDbType.VarChar, -1) { Value = quote.Text },
                    new SqlParameter("AttributedTo", System.Data.SqlDbType.VarChar, 255) { Value = quote.AttributedTo },
                    new SqlParameter("AttributedDate", System.Data.SqlDbType.DateTime) { Value = quote.AttributedDate },
                    new SqlParameter("CreatedBy", System.Data.SqlDbType.VarChar, 255) { Value = quote.CreatedBy }
                };

                comm.Parameters.AddRange(prms);

                comm.ExecuteNonQuery();
            }
        }

        public Quote GetRandom()
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Quote_GetRandom", conn))
            {
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                if (reader.Read())
                {
                    return new Quote
                    {
                        ID = (int)reader["ID"],
                        Text = (string)reader["Text"],
                        AttributedTo = (string)reader["AttributedTo"],
                        AttributedDate = (DateTime)reader["AttributedDate"],
                        CreatedDate = (DateTime)reader["CreatedDate"],
                        CreatedBy = (string)reader["CreatedBy"]
                    };
                }
            }

            return null;
        }

        public Quote GetRandomByName(string name)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Quote_GetRandomByName @Username", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("Username", System.Data.SqlDbType.VarChar, 255) { Value = name },
                };

                comm.Parameters.AddRange(prms);

                SqlDataReader reader = comm.ExecuteReader();

                if (reader.Read())
                {
                    return new Quote
                    {
                        ID = (int)reader["ID"],
                        Text = (string)reader["Text"],
                        AttributedTo = (string)reader["AttributedTo"],
                        AttributedDate = (DateTime)reader["AttributedDate"],
                        CreatedDate = (DateTime)reader["CreatedDate"],
                        CreatedBy = (string)reader["CreatedBy"]
                    };
                }
            }

            return null;
        }
    }
}
