namespace Bug.Bounty.LoginService
{
    using Bug.Bounty.DataClasses;
    using Newtonsoft.Json;
    using System.Net.Http;

    public class BoldService
    {
        private BoldApiClient boldApiClient;

        public BoldService(HttpClient httpClient)
        {
            boldApiClient = new BoldApiClient(httpClient);
        }

        public SyncfusionLoginResponse SyncfusionLogin(SyncfusionLoginRequest loginRequestData)
        {
            var resultObject = boldApiClient.RequestExecutor(HttpMethod.Post, BoldApiEndPoints.SyncfusionLogin, loginRequestData);
            var result = resultObject as HttpResponseMessage;

            return JsonConvert.DeserializeObject<SyncfusionLoginResponse>(result.Content.ReadAsStringAsync().Result);
        }
    }
}
