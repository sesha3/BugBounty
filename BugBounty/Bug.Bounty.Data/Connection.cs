namespace Bug.Bounty.Data
{
    using System;
    using System.Web.Configuration;

    public class Connection
    {
        public static string ConnectionString => "Data Source = " + AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["AppDataPath"] + "BugBounty.sdf; Password = syncfusion";
    }
}