namespace Bug.Bounty.Data
{
    using Bug.Bounty.DataClasses;
    using System;
    using System.Data.SqlServerCe;

    public class DataProvider
    {
        public static Result ExecuteReaderQuery(string query)
        {
            var result = new Result();
            if (!string.IsNullOrWhiteSpace(Connection.ConnectionString))
            {
                var connection = new SqlCeConnection(Connection.ConnectionString);

                var adapter = new SqlCeDataAdapter(query, connection);
                try
                {
                    adapter.Fill(result.DataTable);
                }
                catch (Exception e)
                {
                    //exception handling
                }
                finally
                {
                    adapter.Dispose();
                    connection.Close();
                }
            }
            else
            {
               //exception handling
            }

            result.Status = true;
            return result;
        }

        public static Result ExecuteNonQuery(string query, string connectionString)
        {
            var result = new Result();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = new SqlCeConnection(connectionString);

                var command = new SqlCeCommand(query, connection);
                try
                {
                    command.Connection.Open();
                    result.ReturnValue = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    //exception handling
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            else
            {
                //exception handling
            }

            result.Status = true;
            return result;
        }
    }
}