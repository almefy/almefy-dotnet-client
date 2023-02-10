using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almefy.net.client {
    public class EnrollmentReturn : CustomJavaScriptSerializer {

        [JsonProperty(PropertyName = "id")] 
        public string Id { get; set; }
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty(PropertyName = "expiresAt")]
        public DateTime ExpiresAt { get; set; }
        [JsonProperty(PropertyName = "base64ImageData")]
        public string Base64ImageData { get; set; }
        [JsonProperty(PropertyName = "identity")]
        public IdentityReturn Identity { get; set; }
       
    }
}
