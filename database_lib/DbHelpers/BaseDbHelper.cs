using System.Data.SqlClient;

namespace database_lib.DbHelpers
{
    public abstract class BaseDbHelper
    {
        protected SqlConnection connection;

        public void SetConnectionString(string connectionString)
        {
            connection = new SqlConnection();
            this.connection.ConnectionString = connectionString;
        }
    }
}
