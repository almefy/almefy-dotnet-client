namespace almefy.net.client {
    public class MessageObject : CustomJavaScriptSerializer {

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}
