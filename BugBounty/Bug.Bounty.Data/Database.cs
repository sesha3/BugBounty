using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Web.Configuration;

namespace Bug.Bounty.Data
{
    public class Database
    {
        public  string GenerateDatabase()
        {
            var sqlcedbScript = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["SystemConfigurationPath"] + "\\sql_tables.sql");

            var appDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["AppDataPath"];
            var connStr = "Data Source = " + appDataFolderPath + "BugBounty.sdf; Password = syncfusion";

            if (Directory.Exists(appDataFolderPath + "//BugBounty.sdf") == false)
            {
                Directory.CreateDirectory(appDataFolderPath);
               

                using (var engine = new SqlCeEngine(connStr))
                {
                    engine.CreateDatabase();
                }

                var script = sqlcedbScript.OpenText().ReadToEnd().TrimEnd();

                SqlCeConnection conn = null;

                try
                {
                    conn = new SqlCeConnection(connStr);

                    conn.Open();

                    var cmd = conn.CreateCommand();

                    var splitter = new[] { ";" };

                    var commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string commandText in commandTexts)
                    {
                        cmd.CommandText = commandText;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
                return connStr;
            }
            else
            {
                return connStr;
            }
        }
    }
}