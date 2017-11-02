using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Utilities
{
    //public class MySQLDataAccess
    //{
    //    string _connectionString;

    //    MySQLDataAccess()
    //    {
    //        _connectionString = ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString;
    //    }

    //    public T GetData<T>(string spName, MySqlParameter[] paramColln)
    //    {
    //        using (var conn = new MySqlConnection(_connectionString))
    //        {
    //            using (MySqlCommand cmd = new MySqlCommand())
    //            {
    //                cmd.CommandText = spName;
    //                cmd.Connection = conn;
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.Parameters.AddRange(paramColln);
    //                cmd.e
    //            }
    //        }
    //    }
    //}
}
