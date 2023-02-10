using System.Collections.Generic;

namespace almefy.net.client {
    public class IdentityReturn : CustomJavaScriptSerializer {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "createdAt")]
        public string CreatedAt { get; set; }
        [JsonProperty(PropertyName = "locked")]
        public bool Locked { get; set; }
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }
        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "tokens")]
        public List<TokenReturn> Tokens { get; set; }

    }
}
