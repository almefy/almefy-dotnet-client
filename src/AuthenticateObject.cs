using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace almefy.net.client {
    public class AuthenticateObject : CustomJavaScriptSerializer {

        [JsonProperty("challenge")]
        public string Challenge { get; set; }
        [JsonProperty("otp")]
        public string OTP { get; set; }

    }
}
