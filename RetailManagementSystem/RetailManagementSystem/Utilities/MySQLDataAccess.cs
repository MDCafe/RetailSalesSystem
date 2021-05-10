using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace RetailManagementSystem.Utilities
{
    public static class MySQLDataAccess
    {
        public static MySqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString;
            return new MySqlConnection(connectionString);
        }

        public static EntityConnection GetEntityConnection()
        {
            // Specify the provider name, server and database.
            string providerName = "MySql.Data.MySqlClient";
            string serverName = "WoodlandsTechnologies";
            string databaseName = "RMS";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.PersistSecurityInfo = true;
            sqlBuilder.UserID = "RMS";
            sqlBuilder.Password = "RMS!@#$";

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
            new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/RMSDataModel.csdl|
                        res://*/RMSDataModel.ssdl|
                        res://*/RMSDataModel.msl";

            using (EntityConnection conn =
            new EntityConnection(entityBuilder.ToString()))
            {
                conn.Open();
                // Console.WriteLine("Just testing the connection.");
                conn.Close();
            }
            return new EntityConnection(entityBuilder.ToString());
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
