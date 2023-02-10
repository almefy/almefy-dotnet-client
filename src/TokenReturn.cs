using System;

namespace almefy.net.client {
    public class TokenReturn : CustomJavaScriptSerializer {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
       
    }
}



