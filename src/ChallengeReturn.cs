using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace almefy.net.client {

    public class AuthImageReturn : JavaScriptSerializer {

        [JsonProperty("qrcodeextended")]
        public string QRCodeExtended { get; set; }
        [JsonProperty("qrcodestandard")]
        public string QRCodeStandard { get; set; }
        [JsonProperty("qrcodelowres")]
        public string QRCodeLowRes { get; set; }
    }

    public class ChallengeReturn : JavaScriptSerializer {
        [JsonProperty("authType")]
        public string AuthType { get; set; }
        [JsonProperty("authImages")]
        public AuthImageReturn authImages { get; set; }
        [JsonProperty("authImagesData")]
        public AuthImageReturn authImagesData { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        [JsonProperty("id")] 
        public string Id { get; set; }
        [JsonProperty("pollingUrl")]
        public string PollingUrl { get; set; }
        [JsonProperty("socketsUrl")]
        public string SocketsUrl { get; set; }

    }
}
