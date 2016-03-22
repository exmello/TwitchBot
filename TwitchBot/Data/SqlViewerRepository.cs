using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Data
{
    public class SqlViewerRepository : IViewerRepository
    {
        public void AddUpdateViewer(string user, string channel, string streamID)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Viewer_AddUpdate", conn))
            {
                var prms = new SqlParameter[]
                {
                    new SqlParameter("usernames", System.Data.SqlDbType.VarChar, -1) { Value = user },
                    new SqlParameter("channel", System.Data.SqlDbType.VarChar, 50) { Value = channel },
                    new SqlParameter("streamID", System.Data.SqlDbType.VarChar, 50) { Value = streamID },
                };

                comm.Parameters.AddRange(prms);

                comm.BeginExecuteNonQuery();
            }
        }
        
        public void AddUpdateViewers(IEnumerable<string> users, string channel, string streamID)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_Viewer_AddUpdate", conn))
            {
                string userString = string.Join(";", users);

                var prms = new SqlParameter[]
                {
                    new SqlParameter("usernames", System.Data.SqlDbType.VarChar, -1) { Value = userString },
                    new SqlParameter("channel", System.Data.SqlDbType.VarChar, 50) { Value = channel },
                    new SqlParameter("streamID", System.Data.SqlDbType.VarChar, 50) { Value = streamID },
                };

                comm.Parameters.AddRange(prms);

                comm.BeginExecuteNonQuery();
            }
        }

        public int GetUniqueViewerCount(string channel, string streamID)
        {
            string connString = Config.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand comm = new SqlCommand("sp_GetUniqueViewerCount", conn))
            {
                var prms = new SqlParameter[]
                {
                    new SqlParameter("channel", System.Data.SqlDbType.VarChar, 50) { Value = channel },
                    new SqlParameter("streamID", System.Data.SqlDbType.VarChar, 50) { Value = streamID },
                };

                comm.Parameters.AddRange(prms);

                int count = (int)comm.ExecuteScalar();
                return count;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
