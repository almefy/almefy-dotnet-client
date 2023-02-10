using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almefy.net.client {
    public class ChallengeStart: CustomJavaScriptSerializer {

        public ChallengeStart() {
            AuthType = "jwt";
            ReturnAuthImageData = true;
        }
        [JsonProperty(PropertyName = "authType")]
        public string AuthType { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("returnAuthImageData")]
        public bool ReturnAuthImageData { get; set; }

    }
}
