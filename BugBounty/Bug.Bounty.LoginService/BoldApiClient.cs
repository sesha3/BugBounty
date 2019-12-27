namespace Bug.Bounty.LoginService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Bug.Bounty.DataClasses;
    using Newtonsoft.Json;

    public class BoldApiClient
    {
        private HttpClient _httpClient;

        private static Token OAuthAuthentication;

        public BoldApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private Token GetAccessToken()
        {
            var tokenRequest = new TokenRequest
            {
                GrantType = "client_credentials",
                ClientId = ConfigurationManager.AppSettings["bold:api:client_id"],
                ClientSecret = ConfigurationManager.AppSettings["bold:api:client_secret"]
            };

            var request = new HttpRequestMessage(HttpMethod.Post, BoldApiEndPoints.TokenEndPoint);

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", tokenRequest.GrantType),
                new KeyValuePair<string, string>("client_id", tokenRequest.ClientId),
                new KeyValuePair<string, string>("client_secret", tokenRequest.ClientSecret)
            };

            request.Content = new FormUrlEncodedContent(keyValues);

            var response = _httpClient.SendAsync(request).Result;
            var result = response.Content.ReadAsStringAsync().Result;

            OAuthAuthentication = JsonConvert.DeserializeObject<Token>(result);

            OAuthAuthentication.ExpiresOn = DateTime.UtcNow.AddSeconds(OAuthAuthentication.ExpiresIn - 600);

            return OAuthAuthentication;
        }

        public object RequestExecutor<T>(HttpMethod httpMethod, string endpoint, T data)
        {
            var responseObject = new object();

            try
            {
                if (OAuthAuthentication == null || (OAuthAuthentication != null && OAuthAuthentication.ExpiresOn < DateTime.UtcNow))
                {
                    OAuthAuthentication = GetAccessToken();
                }

                var jsonData = data != null ? data.AsJson() : new StringContent(string.Empty);

                switch (httpMethod.Method)
                {
                    case "POST":
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAuthAuthentication.AccessToken);
                        responseObject = _httpClient.PostAsync(endpoint, jsonData).Result;
                        break;

                    case "GET":
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAuthAuthentication.AccessToken);
                        responseObject = _httpClient.GetStringAsync(endpoint).Result;
                        break;
                }
            }
            catch (Exception ex)
            {
                //LogExtension.LogError(string.Empty, "Error while processing API : " + endpoint, ex, MethodBase.GetCurrentMethod());
                throw;
            }

            return responseObject;
        }

        public async Task<object> RequestExecutorAsync<T>(HttpMethod httpMethod, string endpoint, T data)
        {
            var responseObject = new object();

            try
            {
                if (OAuthAuthentication == null || (OAuthAuthentication != null && OAuthAuthentication.ExpiresOn < DateTime.UtcNow))
                {
                    OAuthAuthentication = GetAccessToken();
                }

                var jsonData = data != null ? data.AsJson() : new StringContent(string.Empty);

                switch (httpMethod.Method)
                {
                    case "POST":
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAuthAuthentication.AccessToken);
                        responseObject = await _httpClient.PostAsync(endpoint, jsonData);
                        break;

                    case "GET":
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAuthAuthentication.AccessToken);
                        responseObject = await _httpClient.GetStringAsync(endpoint);
                        break;
                }
            }
            catch (Exception ex)
            {
                //LogExtension.LogError(string.Empty, "Error while processing API : " + endpoint, ex, MethodBase.GetCurrentMethod());
                throw;
            }

            return responseObject;
        }
    }
}
