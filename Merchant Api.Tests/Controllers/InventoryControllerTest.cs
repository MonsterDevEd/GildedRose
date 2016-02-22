using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Merchant_Api.Controllers;
using Merchant_Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Merchant_Api.Tests.Controllers
{
    [TestClass]
    public class InventoryControllerTest
    {

        private static string SharedSecret;
        private static readonly Random Rnd = new Random();
        private List<Item> Items = new List<Item>();

        [TestInitialize]
        public void Starter()
        {
            SharedSecret = Common.MOCK_SHARED_SECRET;
            var count = Rnd.Next(3, 10);

            for (int i = 1; i < count; i++)
            {
                Items.Add(new Item
                {
                    ItemId = $"Sku-0{i}",
                    Name = $"Gilded Item {i + 1 }",
                    Description = $"Item {i + 1} description",
                    Price = (int)((i + 0.99) * 100)
                });
            }
        }

        [TestMethod]
        public void GetInventoryControllerTest()
        {
            // Arrange
            InventoryController controller = new InventoryController();
            controller.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/inventory/getInventory");
            controller.Configuration = new HttpConfiguration();

            // Act
            var result = controller.GetInventory().Result;
            var inventory = result as OkNegotiatedContentResult<Inventory>;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Inventory>));
            Assert.IsNotNull(inventory);
            CollectionAssert.AllItemsAreNotNull(inventory.Content.Items);

            Console.Write(JsonConvert.SerializeObject(inventory.Content, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        }


        [TestMethod]
        public void CheckInventoryControllerTest()
        {
            // Arrange
            InventoryController controller = new InventoryController();
            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/inventory/checkInventory");
            controller.Configuration = new HttpConfiguration();

            // Act
            var result = controller.CheckInventory(Items).Result;
            var items = result as OkNegotiatedContentResult<List<Item>>;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<Item>>));
            Assert.IsNotNull(items);
            CollectionAssert.AllItemsAreNotNull(items.Content);

            Console.WriteLine(JsonConvert.SerializeObject(items.Content, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            //check badrequest
            var result2 = controller.CheckInventory(new List<Item>()).Result as BadRequestResult;
            Assert.IsNotNull(result2);

        }

        /// <summary>
        /// Successful GET Inventory request
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void GetInventoryHttpSuccess()
        {

            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpGetReqest(new Uri("http://localhost:55774/api/inventory/getinventory/" + Common.MOCK_MERCHANT_TWO), header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            var json = JObject.Parse(responseContent);
            Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
        }

        /// <summary>
        /// Fail authentication
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void GetInventoryHttpFailAuth()
        {
            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, "TU9DS19TSEFSRURfU0VDUkVU");
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpGetReqest(new Uri("http://localhost:55774/api/inventory/getinventory/" + Common.MOCK_MERCHANT_TWO), header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Console.WriteLine(responseContent);
        }

        /// <summary>
        /// CheckInventory invalid method
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void GetInventoryHttpFailMethod()
        {

            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/inventory/getinventory/" + Common.MOCK_MERCHANT_TWO), Items, header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsFalse(string.IsNullOrWhiteSpace(responseContent));
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode);
            Console.WriteLine(responseContent);

        }

        /// <summary>
        /// CheckInventory enpoint
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void CheckInventoryHttpSuccess()
        {

            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/inventory/checkinventory/" + Common.MOCK_MERCHANT_TWO), Items, header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsFalse(string.IsNullOrWhiteSpace(responseContent));
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            var obj = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        /// <summary>
        /// CheckInventory endpoint bad requeset
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void CheckInventoryHttpFail()
        {

            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/inventory/checkinventory/" + Common.MOCK_MERCHANT_TWO), new List<Item>(), header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(string.IsNullOrWhiteSpace(responseContent));
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        }

        /// <summary>
        /// Fail authentication
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void CheckInventoryHttpFailAuth()
        {
            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, "TU9DS19TSEFSRURfU0VDUkVU");
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpPostReqest(new Uri("http://localhost:55774/api/inventory/checkinventory/" + Common.MOCK_MERCHANT_TWO), new List<Item>(), header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Console.WriteLine(responseContent);
        }

        /// <summary>
        /// CheckInventory invalid method
        /// <note type="important">IMPORTANT: Local IIS must be running the web api project (ctrl+ f5)</note>
        /// </summary>
        [TestMethod]
        public void CheckInventoryHttpFailMethod()
        {

            var token = Common.GenerateToken(Common.MOCK_MERCHANT_TWO, Common.MOCK_SHARED_SECRET);
            var header = new AuthenticationHeaderValue("Token", token);
            var result = Common.MakeHttpGetReqest(new Uri("http://localhost:55774/api/inventory/checkinventory/" + Common.MOCK_MERCHANT_TWO), header);
            var responseContent = result.Content.ReadAsStringAsync().Result;
            Assert.IsFalse(string.IsNullOrWhiteSpace(responseContent));
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode);
            Console.WriteLine(responseContent);

        }


        


    }
}
