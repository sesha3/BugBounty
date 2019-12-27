namespace Bug.Bounty.Base
{
    using System;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Web;

    public class GlobalAppSettings
    {
        public static HttpClient SupportTicketClient { get; set; }

        public static HttpClient BoldClient { get; set; }

        public static HttpClient GetSupportClient()
        {
            return SupportTicketClient ?? new TicketHttpClient().GetClient(CloudAppConfig.SyncfusionWebApi);
        }

        public static HttpClient GetBoldClient()
        {
            return BoldClient ?? new TicketHttpClient().GetClient(CloudAppConfig.BoldWebApi);
        }
       
        public virtual bool IsEmail(string email)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                result = Regex.IsMatch(email, expresion) ? true : false;

                if (email.Length > 254 && result)
                {
                    result = false;
                }

                if (email.Length > 95 && !result)
                {
                    result = false;
                }
            }

            return result;
        }

        public static void SetTimeZone(Guid userId = default(Guid))
        {
            try
            {
                if (userId == Guid.Empty && HttpContext.Current != null && HttpContext.Current.User != null
                    && HttpContext.Current.User as ServerPrincipal != null
                    && (HttpContext.Current.User as ServerPrincipal).UserId != Guid.Empty)
                {
                    userId = (HttpContext.Current.User as ServerPrincipal).UserId;
                }
                else if (userId == Guid.Empty && HttpContext.Current != null && HttpContext.Current.Session != null
                         && HttpContext.Current.Session["UserId"] != null)
                {
                    userId = Guid.Parse(HttpContext.Current.Session["UserId"].ToString());
                }

                // Need to update once added usermanagement
                var timeZone = string.Empty;

                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["TimeZone"] = !string.IsNullOrWhiteSpace(timeZone)
                                                                  ? timeZone : "Eastern Standard Time";
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}