using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace almefy.net.client {

    public enum ChallengeTokenStatus {
        Open =1,
        Success = 2,
        NotValid = 3
    }
    public class ChallengeTokenReturn : JavaScriptSerializer {
        [JsonProperty("token")]
        public string Token { get; set; }

        public ChallengeTokenStatus Status { get; set; }
    }
}
