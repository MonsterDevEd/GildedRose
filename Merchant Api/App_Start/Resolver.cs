using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Merchant_Api.Controllers;
using Merchant_Api.Models;

namespace Merchant_Api.App_Start
{
    public class Resolver : IDependencyResolver
    {
        protected static readonly IOrder OrderObj = new OrderModel();
        public object GetService(Type serviceType)
        {
            //return serviceType == typeof(OrderController) ? new OrderController(OrderObj) : null;
            return serviceType;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
            
        }

    }
}
