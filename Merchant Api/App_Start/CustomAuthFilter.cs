using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using JWT;
using Merchant_Api.Models.Auth;

namespace Merchant_Api
{
    /// <summary>
    /// Authentication via JWT
    /// </summary>
    public class JWtHeaderFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var rdata = actionContext.ControllerContext?.RouteData;
            object mid;
            if (rdata == null || !rdata.Values.Any() || !rdata.Values.TryGetValue("merchantId", out mid))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null
                || !authHeader.Scheme.Equals("token", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(authHeader.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }
            var token = authHeader.Parameter;
            var merchant = mid.ToString();

            //This is a mock lookup to obtain a merchant's shared secret
            if (merchant.Equals("MID001", StringComparison.Ordinal) || merchant.Equals("MID002", StringComparison.Ordinal))
            {
                //For mocking only. Merchant lookup from datastore will provide value
                var secret = AuthUtils.MOCK_SHARED_SECRET;

                try
                {
                    JsonWebToken.Decode(token, secret);
                }
                catch (SignatureVerificationException ex)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                    return;
                }
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }



            base.OnAuthorization(actionContext);
        }
    }
}