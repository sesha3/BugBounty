namespace hackathon_2019.Models
{
    using Bug.Bounty.Base;
    using Bug.Bounty.DataClasses;
    using Bug.Bounty.LoginService;
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Security;

    public class AccountModel
    {
        private readonly BoldService boldService;

        public AccountModel()
        {
            boldService = new BoldService(GlobalAppSettings.GetBoldClient());
        }

        public SyncfusionLoginResponse ValidateSyncfusionUser(string email, string password, string remember, string ipAddress, string host, string clientId = null)
        {
            var response = new SyncfusionLoginResponse();

            try
            {
                var syncfusionLoginRequest = new SyncfusionLoginRequest
                {
                    CustomerEmail = email.Trim(),
                    Password = password
                };

                var syncfusionUserDetails = boldService.SyncfusionLogin(syncfusionLoginRequest);

                if (syncfusionUserDetails == null || syncfusionUserDetails.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    response.Status = false;
                    response.Message = "connection issue";

                    return response;
                }

                if (!syncfusionUserDetails.IsActivated)
                {
                    response.Status = syncfusionUserDetails.IsActivated;
                    response.Message = syncfusionUserDetails.Message;
                    response.CustomerEmail = syncfusionUserDetails.CustomerEmail;

                    return response;
                }

                var userData = new BugManagement().GetUser(syncfusionUserDetails.CustomerEmail);

                if (syncfusionUserDetails.StatusCode == (int)HttpStatusCode.OK && syncfusionUserDetails.IsActivated)
                {
                    if (userData == null)
                    {
                        var user = new User
                        {
                            DisplayName = syncfusionUserDetails.CustomerName,
                            Email = syncfusionUserDetails.CustomerEmail,
                            Role = UserRole.Engineer,
                            Platform = Platform.BoldBI,
                            IsActive = true,
                            IsDeleted = false,
                        };

                        userData = new BugManagement().AddUser(user);

                        response.HavePrivacyConsent = false;
                    }

                    SetLoginCookies(userData, remember != null && remember.ToLower().Trim() == "true", ipAddress, host, clientId);

                    response.Message = syncfusionUserDetails.Message;
                    response.CustomerEmail = userData.Email;
                    response.Status = true;
                }
            }
            catch (Exception e)
            {
                //LogExtension.LogError(string.Empty, $"Error while validate syncfusion user. Email: {email}", exception, MethodBase.GetCurrentMethod());
            }

            return response;
        }

        public void SetLoginCookies(User userDetail, bool isPersistent, string ipAddress = null, string host = null, string clientId = null)
        {
            FormsAuthentication.SetAuthCookie(userDetail.Email, isPersistent);

            GlobalAppSettings.SetTimeZone(userDetail.Id);
            UpdateSession(userDetail);
            if (clientId != null)
            {
                host = HttpContext.Current.Request.UrlReferrer == null ? string.Empty : HttpContext.Current.Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
            }
        }

        public void UpdateSession(User userInfo)
        {
            HttpContext.Current.Session["displayname"] = userInfo.DisplayName;
            HttpContext.Current.Session["IsAdmin"] = userInfo.Role == UserRole.PlatformManager || userInfo.Role == UserRole.Management;
            HttpContext.Current.Session["emailaddress"] = userInfo.Email;
        }
    }
}