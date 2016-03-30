using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public class SqlViewerRepository : IViewerRepository
    {
        public void AddUpdateViewer(string user, string channel, string streamID)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Viewer_AddUpdate @usernames, @channel, @streamID", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("usernames", System.Data.SqlDbType.VarChar, -1) { Value = user },
                    new SqlParameter("channel", System.Data.SqlDbType.VarChar, 50) { Value = channel },
                    new SqlParameter("streamID", System.Data.SqlDbType.VarChar, 50) { Value = streamID },
                };

                comm.Parameters.AddRange(prms);

                comm.ExecuteNonQuery();
            }
        }
        
        public void AddUpdateViewers(IEnumerable<string> users, string channel, string streamID)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Viewer_AddUpdate @usernames, @channel, @streamID", conn))
            {
                conn.Open();

                string userString = string.Join(";", users);

                var prms = new SqlParameter[]
                {
                    new SqlParameter("usernames", System.Data.SqlDbType.VarChar, -1) { Value = userString },
                    new SqlParameter("channel", System.Data.SqlDbType.VarChar, 50) { Value = channel },
                    new SqlParameter("streamID", System.Data.SqlDbType.VarChar, 50) { Value = streamID },
                };

                comm.Parameters.AddRange(prms);

                comm.ExecuteNonQuery();
            }
        }

        public int GetUniqueViewerCount(string channel, string streamID)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_GetUniqueViewerCount @streamID", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("streamID", System.Data.SqlDbType.VarChar, 50) { Value = streamID },
                };

                comm.Parameters.AddRange(prms);

                SqlDataReader reader = comm.ExecuteReader();

                if(reader.Read())
                {
                    return (int)reader[0];
                }

                return 0;
            }
        }

        public void AddUpdateBnet(string user, string bnet)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_ViewerInfo_Bnet_AddUpdate @Username, @Bnet", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("Username", System.Data.SqlDbType.VarChar, 255) { Value = user },
                    new SqlParameter("Bnet", System.Data.SqlDbType.VarChar, 50) { Value = bnet }
                };

                comm.Parameters.AddRange(prms);

                comm.ExecuteNonQuery();
            }
        }

        public void AddUpdateNote(string user, string note)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_ViewerInfo_Note_AddUpdate @Username, @Note", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("Username", System.Data.SqlDbType.VarChar, 255) { Value = user },
                    new SqlParameter("Note", System.Data.SqlDbType.VarChar, -1) { Value = note }
                };

                comm.Parameters.AddRange(prms);

                comm.ExecuteNonQuery();
            }
        }

        public ViewerInfo GetInfo(string user)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_ViewerInfo_Get @username", conn))
            {
                conn.Open();

                var prms = new SqlParameter[]
                {
                    new SqlParameter("Username", System.Data.SqlDbType.VarChar, 255) { Value = user },
                };

                comm.Parameters.AddRange(prms);

                SqlDataReader reader = comm.ExecuteReader();

                if (reader.Read())
                {
                    return new ViewerInfo
                    {
                        Username = (string)reader["Username"],
                        Bnet = reader["Bnet"] != DBNull.Value ? (string)reader["Bnet"] : null,
                        Note = reader["Note"] != DBNull.Value ? (string)reader["Note"] : null,
                    };
                }

                return null;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
