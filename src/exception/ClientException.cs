using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace almefy.net.client.exception {
    /// <summary>
    /// AlmefyAPIClientException is the base-class all logic 
    /// exceptions thrown by almefy-net-client.
    /// </summary>
    [Serializable]
    public class ClientException : Exception {
        internal ClientException() : base() { }
        internal ClientException(string msg) : base(msg) { }
#if (DEBUG)
        internal ClientException(string msg, Exception e) : base(msg, e) { }
#else
        internal ClientException(string msg, Exception e) : base(msg){}
#endif

        //Deserialization constructor, needed since our base class implements ISerializable
        protected ClientException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

    }
}
