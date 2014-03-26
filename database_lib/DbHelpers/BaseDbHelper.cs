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


        protected int ExecuteNonQueryCommand(SqlCommand command)
        {
            int affectedRows = 0;
            try
            {
                connection.Open();
                //bing opened connection to the command
                command.Connection = connection;

                affectedRows = command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            { }
            finally
            {
                connection.Close();
            }

            return affectedRows;
        }

        protected object ExecuteScalarCommand(SqlCommand command)
        {
            object result = null;

            try
            {
                connection.Open();
                //bing opened connection to the command
                command.Connection = connection;

                result = command.ExecuteScalar();
            }
            catch (SqlException ex)
            { }
            finally
            {
                connection.Close();
            }

            return result;
        }
    }
}
