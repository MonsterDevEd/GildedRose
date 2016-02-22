using System.Collections.Generic;
using System.Web.Http;
using Merchant_Api.Models;
using Merchant_Api.Models.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Merchant_Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //JSON serialization settings handling, adjust as necessary
            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //camel case
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //omit null
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            
            //configure JWT
            JWT.JsonWebToken.JsonSerializer = new NewtonJsonSerializer();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            
        }
    }
}
