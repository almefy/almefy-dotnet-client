using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almefy.net.client {
    public class EnrollIdentity : CustomJavaScriptSerializer {
        public EnrollIdentity() {

            Identifier = String.Empty; //HINT ... String.Empty is needed because API Endpoint can't handle null
            Nickname = String.Empty; //HINT ... String.Empty is needed because API Endpoint can't handle null
            SendEmailTo = String.Empty; //HINT ... String.Empty is needed because API Endpoint can't handle null
            SendEmail = false;
            EnrollmentType = AlmefyAPIClient.ONE_STEP_ENROLLMENT; //FIXME rework to struct
            SendEmailLocale = "en_US"; //FIXME rework

        }

        [JsonProperty(PropertyName = "enrollmentType")]
        public string EnrollmentType { get; set; }
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }
        [JsonProperty(PropertyName = "sendEmail")]
        public bool SendEmail { get; set; }
        [JsonProperty(PropertyName = "sendEmailTo")]
        public string SendEmailTo { get; set; }
        [JsonProperty(PropertyName = "sendEmailLocale")]
        public string SendEmailLocale { get; set; }
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }



    }
}
