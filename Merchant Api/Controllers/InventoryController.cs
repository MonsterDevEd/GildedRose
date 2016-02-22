using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Merchant_Api.Models;
using System.Linq;

namespace Merchant_Api.Controllers
{
    /// <summary>
    /// Merchant Inventory API
    /// </summary>
    [JWtHeaderFilter]
    public class InventoryController : ApiController
    {

        /// <summary>
        /// Obtain Gilded Rose Inventory
        /// </summary>
        /// <returns>Gilded Rose Inventory</returns>
        [Route("~/api/inventory/getInventory/{merchantId}")]
        [ResponseType(typeof(Inventory))]
        public async Task<IHttpActionResult> GetInventory()
        {
            return Ok(await BuildInventoryData());
        }

        /// <summary>
        /// Check availability of items
        /// </summary>
        /// <param name="items">List of items to check</param>
        /// <returns></returns>
        [Route("~/api/inventory/checkInventory/{merchantId}")]
        [ResponseType(typeof(List<Item>))]
        public async Task<IHttpActionResult> CheckInventory(List<Item> items)
        {
            if (items == null || !items.Any())
                return BadRequest();

            return Ok(await CheckAvailability(items));
        }

        /// <summary>
        /// Mock
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private async Task<List<Item>> CheckAvailability(List<Item> items)
        {
            
            await Task.Factory.StartNew(() =>
            {
                var count = items.Count;
                for (int i = 0; i < count; i++)
                {
                    items[i].InStock = i%2 != 0;
                }

            });

            return items;
        }

        /// <summary>
        /// Mock
        /// </summary>
        /// <returns></returns>
        private async Task<Inventory> BuildInventoryData()
        {
            var items = new List<Item>();

            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    items.Add(new Item
                    {
                        ItemId = $"Sku-0{i}",
                        Name = $"Gilded Item {i + 1 }",
                        Description = $"Item {i + 1} description",
                        Price = (int)((i + 0.99) * 100),
                        InStock = i % 2 == 0
                    });
                }
            });

            return new Inventory
            {
                Items = items
            };
        }
    }
}
