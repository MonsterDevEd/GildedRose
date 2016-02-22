using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using Merchant_Api.Models;
using Merchant_Api.Tests.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Merchant_Api.Controllers.Tests
{
    [TestClass]
    public class OrderControllerTests
    {
        private OrderModel Order;
        private OrderModel FailOrder;
        private static readonly Random Rnd = new Random();



        [TestInitialize]
        public void BuildOrder()
        {

            Order = new OrderModel
            {
                MerchantId = Common.MOCK_MERCHANT_ONE,
                MerchantOrderReference = Rnd.Next(1000, 2000).ToString(),
                OrderId = Rnd.Next(10000, 20000).ToString(),
                OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                OrderItems = new List<OrderItem>()
            };

            //No merchant id and order items and will fail model validation
            FailOrder = new OrderModel
            {
                MerchantOrderReference = Rnd.Next(1000, 2000).ToString(),
                OrderId = Rnd.Next(10000, 20000).ToString(),
                OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var items = Rnd.Next(3, 10);

            for (int i = 1; i < items; i++)
            {
                Order.OrderItems.Add(new OrderItem
                {
                    Name = $"Gilded Item {i + 1 }",
                    Description = $"Item {i + 1} description",
                    Price = (int)((i + 0.99) * 100),
                    QtyOrdered = i
                });
            }
        }


        [TestMethod]
        public void OrderControllerTest()
        {
            // Arrange
            OrderController controller = new OrderController();
            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/Order/888");
            controller.Configuration = new HttpConfiguration();

            // Act
            var response = controller.Post(Order).Result; // as NegotiatedContentResult<OrderResponse>; //CreatedNegotiatedContentResult<OrderResponse>;

            // Assert
            Assert.IsNotNull(response);

            OrderResponse result = null;
            var ncr = response as NegotiatedContentResult<OrderResponse>;
            if (ncr != null)
            {
                result = ncr.Content;
            }
            else
            {
                var cnr = response as CreatedNegotiatedContentResult<OrderResponse>;
                if (cnr != null) result = cnr.Content;
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(Order.GetGrandTotal(), result.SubmittedOrder.GetGrandTotal());
            Assert.IsInstanceOfType(result.SubmittedOrder, typeof(OrderModel));
            var ordr = result.SubmittedOrder as OrderModel;
            Assert.IsNotNull(ordr);
            Assert.AreEqual(Order.OrderItems.Count, ordr.OrderItems.Count);
            CollectionAssert.AllItemsAreInstancesOfType(ordr.OrderItems, typeof(OrderItem));
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

        }


        /// <summary>
        /// Valid Order submission
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void OrderPostHttpSuccessTest()
        {
            var token = Common.GenerateToken(Common.MOCK_MERCHANT_ONE, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/order/" + Common.MOCK_MERCHANT_ONE), Order, header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(responseContent);
            //conflict invoked randomly by api
            Assert.IsTrue(HttpStatusCode.Created == result.StatusCode || HttpStatusCode.Conflict == result.StatusCode);
            var json = JObject.Parse(responseContent);
            Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
        }

        /// <summary>
        /// Test Invalid Model. BadRequest
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void OrderPostHttpFailModelValidationTest()
        {
            var token = Common.GenerateToken(Common.MOCK_MERCHANT_ONE, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/order/" + Common.MOCK_MERCHANT_ONE), FailOrder, header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            var json = JObject.Parse(responseContent);
            Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
        }

        /// <summary>
        /// Fail authentication
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void OrderPosHttpFailAuth()
        {
            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, "TU9DS19TSEFSRURfU0VDUkVU");
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/order/" + Common.MOCK_MERCHANT_TWO), Order, header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Console.WriteLine(responseContent);
        }

        /// <summary>
        /// Test invalid method (GET)
        /// </summary>
        [TestMethod]
        public void OrderPostHttpFailMethod()
        {

            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpGetReqest(new Uri("http://localhost:55774/api/order/" + Common.MOCK_MERCHANT_TWO), header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsFalse(string.IsNullOrWhiteSpace(responseContent));
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode);
            Console.WriteLine(responseContent);

        }

    }
}