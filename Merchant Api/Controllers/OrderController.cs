using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Merchant_Api.Models;

namespace Merchant_Api.Controllers
{
    /// <summary>
    /// Merchant Ordering Apis
    /// </summary>
    [JWtHeaderFilter]
    public class OrderController : ApiController
    {
        /// <summary>
        /// Submit an order for processing
        /// </summary>
        /// <param name="order">Order data</param>
        /// <returns>Result of order submission</returns>
        [ResponseType(typeof(OrderResponse))]
        [Route("~/api/order/{merchantId}")]
        public async Task<IHttpActionResult> Post(OrderModel order)
        {
            if (order != null && ModelState.IsValid)
            {
                try
                {
                    var validated = await ValidateOrder(order);

                    if (validated)
                    {
                        return Created(Request.RequestUri, new OrderResponse
                        {
                            Status = StatusEnum.Success.ToString(),
                            TransactionId = $"{order.MerchantOrderReference}-TID{new Random().Next(100, 999)}",
                            Message = $"Successfully recieved {order.MerchantOrderReference}, dated {DateTimeOffset.FromUnixTimeSeconds(order.OrderDate.Value).LocalDateTime}, valued at {order.GetGrandTotal()}",
                            SubmittedOrder = order
                        });
                    }

                    return new NegotiatedContentResult<OrderResponse>(HttpStatusCode.Conflict, new OrderResponse
                    {
                        Status = StatusEnum.Error.ToString(),
                        Message = "The order you submitted contains errors (e.g. pricing) or may have backordered items",
                        SubmittedOrder = order
                    }, this);
                }
                catch
                {
                    return InternalServerError(new Exception("We are unable to process your order submission at this time"));
                }
                
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Mock. Some process that validates order -  e.g. price check, backorderd items, etc 
        /// </summary>
        /// <param name="order">Submitted order to evaluate</param>
        /// <returns></returns>
        private async Task<bool> ValidateOrder(IOrder order)
        {
            bool result = false;

            await Task.Factory.StartNew(() =>
            {
                if (order.GetGrandTotal() > 800)
                    result = true;
            });

            return result;
        }
    }
}
