namespace Bug.Bounty.DataClasses
{
    using System;

    using Newtonsoft.Json;

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public DateTime ExpiresOn { get; set; }
    }
}
