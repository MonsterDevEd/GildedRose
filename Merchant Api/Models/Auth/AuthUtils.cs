using System.Net.Http;
using System.Threading.Tasks;
using JWT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Merchant_Api.Models.Auth
{

    /// <summary>
    /// Authentication utilities
    /// </summary>
    internal class AuthUtils
    {

#if DEBUG
        /// <summary>
        /// For testing purposes only in lieu of a data store containing Merchant data
        ///     Each Merchant must have their own shared secret used for signing
        /// </summary>
        internal const string MOCK_SHARED_SECRET = "VGhlIEdpbGRlZCBSb3NlIEFwaQ";
#endif

        /// <summary>
        /// If end users (customers of Merchants) also need to be authenticated, options are:
        ///     Gilded Rose maintains this "directory" of users
        ///     Cede authentication to 3rd party, such as Google     
        /// </summary>
        internal static readonly string GoogleTokenVerifyBaseUrlQuery = @"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=";
        internal static readonly string GoogleClientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];


        /// <summary>
        /// Sample Google login
        /// </summary>
        /// <param name="tokenToVerify">Google Login Token</param>
        /// <param name="userId">We're only using this to validate against response from Google</param>
        /// <returns>Result of authentication check</returns>
        internal static async Task<bool> VerifySocialTokenAsync(string tokenToVerify, string userId)
        {
            if (string.IsNullOrWhiteSpace(tokenToVerify)
                || string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(GoogleClientId))
                return false;

            var str = await VerifyRequestAsync(GoogleTokenVerifyBaseUrlQuery, tokenToVerify);
            if (string.IsNullOrWhiteSpace(str)) return false;
            var respObj = JObject.Parse(str);
            if (respObj == null || !respObj.HasValues) return false;
            bool _verifiedEmail = false;
            if (respObj["email_verified"] != null)
            {
                bool.TryParse(respObj["email_verified"].ToString(), out _verifiedEmail);
            }
            var isVerified = _verifiedEmail && respObj["aud"].ToString() == GoogleClientId && respObj["sub"].ToString() == userId;
            return isVerified;
        }

        /// <summary>
        /// Simple Gooogle verification
        /// </summary>
        /// <param name="baseUrl">Google verification url</param>
        /// <param name="data">data to verify (jwt)</param>
        /// <returns></returns>
        private static async Task<string> VerifyRequestAsync(string baseUrl, string data)
        {

            using (var http = new HttpClient())
            {
                try
                {
                    return await http.GetStringAsync(baseUrl + data);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }



    }




    /// <summary>
    /// Custom serializer for JWT
    /// </summary>
    internal class NewtonJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}