using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace RetailManagementSystem.Utilities
{
    public static class MySQLDataAccess
    {
        public static MySqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString;
            return new MySqlConnection(connectionString);
        }

        public static object GetData(string spName, MySqlParameter[] paramColln)
        {
            using (var conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = spName;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(paramColln);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }


        public static object GetData(string spName)
        {
            using (var conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = spName;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;                    
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }

}
