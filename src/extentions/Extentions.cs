using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace almefy.net.client.src.extentions {
    internal static class Extensions {

        internal static bool IsUriValid(this string uri) {

            if (Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                return true;
            else
                return false;
        }

        internal static string ContentToString(this HttpContent httpContent) {
            var readAsStringAsync = httpContent.ReadAsStringAsync();
            return readAsStringAsync.Result;
        }
    }
}
