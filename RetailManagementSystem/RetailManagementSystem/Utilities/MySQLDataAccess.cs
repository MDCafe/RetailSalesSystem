using MySql.Data.MySqlClient;
using System.Configuration;

namespace RetailManagementSystem.Utilities
{
    public static class MySQLDataAccess
    {
        public static MySqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString;
            return new MySqlConnection(connectionString);
        } 

        //public T GetData<T>(string spName, MySqlParameter[] paramColln)
        //{
        //    using (var conn = new MySqlConnection(_connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand())
        //        {
        //            cmd.CommandText = spName;
        //            cmd.Connection = conn;
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddRange(paramColln);
        //            cmd.e
        //        }
        //    }
        //}
    }
}
