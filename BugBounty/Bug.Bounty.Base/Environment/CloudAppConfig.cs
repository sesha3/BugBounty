namespace Bug.Bounty.Base
{
    using System.Configuration;

    /// <summary>
    /// Returns the data values from web.config for configuring the application
    /// </summary>
    public class CloudAppConfig
    {
        /// <summary>
        /// Website team url for location service
        /// </summary>
        public static string SyncfusionWebApi => ConfigurationManager.AppSettings["syncfusion:api"];

        /// <summary>
        /// Website team url for location service for boldbi
        /// </summary>
        public static string BoldWebApi => ConfigurationManager.AppSettings["bold:api"];

        /// <summary>
        /// Default Login logo url
        /// </summary>
        public static string FallbackLoginLogoUrl => "~/content/images/bounty/application/loginlogo.png";

        /// <summary>
        /// Login logo url
        /// </summary>
        public static string SyncfusionLoginLogoUrl => "https://cdn.boldbi.com/static/company/syncfusion/logo/v1/login_logo.svg";
    }
}