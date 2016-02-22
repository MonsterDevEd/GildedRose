using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using JWT;

namespace Merchant_Api.Tests.Controllers
{
    public class Common
    {
        public const string MOCK_SHARED_SECRET = "VGhlIEdpbGRlZCBSb3NlIEFwaQ";
        public const string MOCK_MERCHANT_ONE = "MID001";
        public const string MOCK_MERCHANT_TWO = "MID002";

        public static HttpResponseMessage MakeHttpPostReqest(Uri target, object o, AuthenticationHeaderValue authHeader = null)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;
                result = client.PostAsJsonAsync(target, o, CancellationToken.None).Result;
            }

            return result;
        }

        public static HttpResponseMessage MakeHttpGetReqest(Uri target, AuthenticationHeaderValue authHeader = null)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;
                result = client.GetAsync(target).Result;
            }

            return result;
        }

        public static string GenerateToken(string merchantId, string key)
        {
            var payload = new Dictionary<string, object>
            {
                {"iss", merchantId},
                {"aud", "The Guilded Rose" },
                {"iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                {"exp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 300 }
            };
            return JsonWebToken.Encode(payload, key, JwtHashAlgorithm.HS256);
        }

    }
}
