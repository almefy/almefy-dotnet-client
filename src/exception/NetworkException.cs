using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace almefy.net.client.exception {
    /// <summary>
    /// NetworkException will be thrown in case of connection issues to almefy-api
    /// </summary>
    [Serializable]
    public class NetworkException : ClientException {
        public HttpStatusCode StatusCode { get; }

        internal NetworkException(string msg) : base(msg) { }
        internal NetworkException(string msg, HttpStatusCode statusCode) : base(msg) { StatusCode = statusCode; }
#if (DEBUG)

        internal NetworkException(string msg, Exception e) : base(msg, e) { }
        internal NetworkException(string msg, HttpStatusCode statusCode, Exception e) : base(msg, e) { StatusCode = statusCode; }
#else
        internal NetworkException(string msg, HttpStatusCode statusCode, Exception e) : base(msg){ StatusCode = statusCode ;}
#endif

    }
}
