namespace Bug.Bounty.Base
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class TicketHttpClient
    {
        private HttpClientHandler _sharedHandler = new HttpClientHandler();

        public HttpClient GetClient(string baseAddress)
        {
            try
            {
                var httpClient = new HttpClient(_sharedHandler, disposeHandler: false)
                {
                    BaseAddress = new Uri(baseAddress),
                    Timeout = TimeSpan.FromSeconds(90)
                };

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return httpClient;
            }
            catch
            {
                return null;
            }
            
        }
    }
}
