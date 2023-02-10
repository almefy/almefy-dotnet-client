using System;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Net;
using almefy.net.client.exception;
using almefy.net.client.src;
using almefy.net.client.src.extentions;

namespace almefy.net.client {

    public class AlmefyAPIClient {

        private readonly HttpClient httpClient;
        private object apiCache;
        private string apiBaseUrl;
        private string apiKey;
        private string apiSecretBase64;

        private const string VERSION = "0.0.1";
        private const string ALMEFY_CHALLENGE = "/v1/entity/challenges";
        private const string ALMEFY_CHECK = "/v1/entity/check";
        private const string ALMEFY_IDENTITIES = "/v1/entity/identities";
        private const string ALMEFY_ENROLLMENTS = "/v1/entity/identities/enroll";
        private const string ALMEFY_TOKENS = "/v1/entity/tokens";
        private const string ALMEFY_AUTHENTICATE = "/v1/entity/identities/{identity}/authenticate";

        private const int REQUEST_TIMESTAMP_LEEWAY = 10;
        public const string ONE_STEP_ENROLLMENT = "ONE_STEP_ENROLLMENT";
        public const string TWO_STEP_ENROLLMENT = "TWO_STEP_ENROLLMENT";

        public AlmefyAPIClient(string apiBaseUrl, string apiKey, string apiSecretBase64, bool checkApi = false, TimeSpan requestTimeout=default(TimeSpan)) {

            if (apiBaseUrl.IsNullOrEmpty())
                throw new ArgumentNullException("apiBaseUrl", "cannot be null or empty");

            if (!apiBaseUrl.IsUriValid())
                throw new ArgumentOutOfRangeException("apiBaseUrl", "is not a valid Uri");

            if (apiKey.IsNullOrEmpty())
                throw new ArgumentNullException("apiKey", "cannot be null or empty");
            if (apiSecretBase64.IsNullOrEmpty())
                throw new ArgumentNullException("apiSecretBase64", "cannot be null or empty");

            this.apiCache = new { };
            this.apiBaseUrl = apiBaseUrl;
            this.apiKey = apiKey;
            this.apiSecretBase64 = apiSecretBase64;
            if (requestTimeout==default(TimeSpan))
                requestTimeout = TimeSpan.FromSeconds(5);

            this.httpClient = new HttpClient {
                BaseAddress = new Uri(this.apiBaseUrl),
                Timeout = requestTimeout
            };

            if (checkApi && !this.Check())
                throw new NetworkException($"cannot reach api-server {this.apiBaseUrl} with given credentials", HttpStatusCode.NotFound);

        }

        private async Task<HttpResponseMessage> CreateApiRequest(HttpMethod method, string url, string bodyJson = "{}", bool signToken = true) {

            HttpRequestMessage hrm = new HttpRequestMessage(method, url);
            if (signToken) {
                string signedToken = CreateApiToken(method.ToString().ToUpper(), url, bodyJson);
                hrm.Headers.Add("Authorization", $"Bearer {signedToken}");
            }
            hrm.Headers.Add("User-Agent", $"Almefy C# Client {VERSION} (NET Environment {Environment.Version})");
            hrm.Headers.Add("X-Client-Version", $"{VERSION}");

            if (method != HttpMethod.Get)
                hrm.Content = new StringContent(bodyJson);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            try {
                var response = await httpClient.SendAsync(hrm);

                if ((int)response.StatusCode >= 500 ) {
                    throw new ServerException("CreateApiRequest", response.StatusCode);
                }
                return response;
            } catch (System.Net.Http.HttpRequestException hex) {
                throw new NetworkException("CreateApiRequest - general error", hex);
            } catch (Exception ex) {

                throw new ClientException("CreateApiRequest", ex);

            }

        }

        private string CreateApiToken(string method, string url, string bodyJson = "{}") {

            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Iat, ((int)now.TotalSeconds).ToString(), ClaimValueTypes.Integer),
                    new Claim("method", method),
                    new Claim("url", this.apiBaseUrl + url),
                    new Claim("bodyHash", ComputeSHA256(bodyJson))
            };
            var ssk = new SymmetricSecurityKey(Convert.FromBase64String(this.apiSecretBase64));
            var sk = new SigningCredentials(ssk, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(this.apiKey, this.apiBaseUrl, claims, DateTime.Now, DateTime.Now.AddSeconds(10), sk);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;

        }

        public bool Check() {
            var bodyJson = new MessageObject { Message = "ping" };
            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Post, ALMEFY_CHECK, CustomJavaScriptSerializer.JsonSerialize(bodyJson)));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            if (t.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                var result = CustomJavaScriptSerializer.JsonDeserialize<MessageObject>(t.Result.Content.ContentToString());
                if (!result.Message.IsNullOrEmpty() && result.Message.ToLower() == "pong")
                    return true;
            }
            return false;
        }

        public List<IdentityReturn> GetIdentities() {

            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Get, $"{ALMEFY_IDENTITIES}", String.Empty));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            if (t.Result.StatusCode == System.Net.HttpStatusCode.OK || t.Result.StatusCode == System.Net.HttpStatusCode.Created) {

                var result = CustomJavaScriptSerializer.JsonDeserialize<List<IdentityReturn>>(t.Result.Content.ContentToString());
                return result;

            }

            return null;

        }

        public IdentityReturn GetIdentity(string identifier) {

            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Get, $"{ALMEFY_IDENTITIES}/{Uri.EscapeUriString(identifier)}", String.Empty));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            if (t.Result.StatusCode == System.Net.HttpStatusCode.OK || t.Result.StatusCode == System.Net.HttpStatusCode.Created) {

                var result = CustomJavaScriptSerializer.JsonDeserialize<IdentityReturn>(t.Result.Content.ContentToString());
                return result;

            }

            return null;

        }
        public EnrollmentReturn EnrollIdentity(string identifier, EnrollIdentity enrollIdentity) {

            enrollIdentity.Identifier = identifier;
            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Post, ALMEFY_ENROLLMENTS, CustomJavaScriptSerializer.JsonSerialize(enrollIdentity)));

            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            if (t.Result.StatusCode == System.Net.HttpStatusCode.OK || t.Result.StatusCode == System.Net.HttpStatusCode.Created) {

                var result = CustomJavaScriptSerializer.JsonDeserialize<EnrollmentReturn>(t.Result.Content.ContentToString());
                return result;

            }

            return null;

        }

        public EnrollmentReturn ProvisionIdentity(string identifier, EnrollIdentity options) {

            return this.EnrollIdentity(identifier, options);

        }

        public bool DeleteIdentity(string identifier) {

            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Delete, $"{ALMEFY_IDENTITIES}/{Uri.EscapeUriString(identifier)}", String.Empty));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            return (t.Result.StatusCode == System.Net.HttpStatusCode.Created || t.Result.StatusCode == System.Net.HttpStatusCode.OK);

        }

        public bool DeleteToken(string id) {

            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Delete, $"{ALMEFY_TOKENS}/{Uri.EscapeUriString(id)}", String.Empty));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;
            
            return (t.Result.StatusCode == System.Net.HttpStatusCode.Created || t.Result.StatusCode == System.Net.HttpStatusCode.OK);

        }

        public string VerifyToken(string token) {

            return this.Authenticate(token);

        }

        public ChallengeReturn CreateChallenge() {

            var challengeStart = new ChallengeStart { Key = apiKey };
            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Post, ALMEFY_CHALLENGE, CustomJavaScriptSerializer.JsonSerialize(challengeStart), false));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            if (t.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                var resultJson = t.Result.Content.ContentToString();
                resultJson = resultJson
                    .Replace("\"qrcode-standard\"", "\"qrcodestandard\"")
                    .Replace("\"qrcode-extended\"", "\"qrcodeextended\"")
                    .Replace("\"qrcode-lowres\"", "\"qrcodelowres\""); //FIXME - workaround, to get serializer to run currently, ignoring hypen currently - rework

                var result = CustomJavaScriptSerializer.JsonDeserialize<ChallengeReturn>(resultJson);
                return result;

            }

            return null;

        }

        public ChallengeTokenReturn CheckChallenge(string pollingUrl) {

            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Get, pollingUrl, "{}", false));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            if (t.Result.StatusCode == System.Net.HttpStatusCode.NoContent) {
                return new ChallengeTokenReturn { Status = ChallengeTokenStatus.Open, Token = "" };
            } else if (t.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                var result = CustomJavaScriptSerializer.JsonDeserialize<ChallengeTokenReturn>(t.Result.Content.ContentToString());
                result.Status = ChallengeTokenStatus.Success;
                return result;
            } else {
                var result = t.Result.Content.ContentToString();

            }

            return new ChallengeTokenReturn { Token = "", Status = ChallengeTokenStatus.NotValid };
        }

        public string Authenticate(string token) {

            JwtSecurityToken jwtSecToken = (JwtSecurityToken)ValidateToken(token);

            var sub = jwtSecToken.Payload["sub"].ToString();
            var identity = jwtSecToken.Payload["jti"].ToString();
            var otp = jwtSecToken.Payload["otp"].ToString();

            string authenticateUrl = ALMEFY_AUTHENTICATE.Replace("{identity}", sub);
            var challengeObject = new AuthenticateObject { Challenge = identity, OTP = otp };
            var t = Task.Run(() => this.CreateApiRequest(HttpMethod.Post, authenticateUrl, CustomJavaScriptSerializer.JsonSerialize(challengeObject)));
            t.Wait();

            if (t.IsFaulted)
                throw t.Exception;

            return (t.Result.StatusCode == System.Net.HttpStatusCode.OK) ? sub : null;

        }
        private SecurityToken ValidateToken(string token) {

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidIssuer = this.apiKey,
                ValidAudience = this.apiBaseUrl,
                //LifetimeValidator = LifetimeValidator,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(this.apiSecretBase64))
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            return validatedToken;

        }
        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params) {
            if (expires != null) {
                return expires > DateTime.UtcNow.AddSeconds(REQUEST_TIMESTAMP_LEEWAY);
            }
            return false;
        }

        static string ComputeSHA256(string s) {
            StringBuilder hash = new StringBuilder();
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                foreach (byte b in hashValue) {
                    hash.Append($"{b:X2}");
                }
            }
            return hash.ToString().ToLower();
        }

    }
}