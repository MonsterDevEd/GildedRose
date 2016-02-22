using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Merchant_Api.Models
{
    /// <summary>
    /// Inventory Object
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// List of Items
        /// </summary>
        public List<Item> Items { get; set; }
    }

    /// <summary>
    /// An Inventory Item
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Item Name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Item Description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Item Price
        /// <note>Because this is an <c>int</c> this represents price in cents</note>
        /// </summary>
        [Required]
        public int Price { get; set; }

        /// <summary>
        /// While not in model definition, highly recommended
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Whether or not an item is available for ordering at time of merchant Inventory API request
        /// </summary>
        public bool? InStock { get; set; }

        /// <summary>
        /// Current stock level of item at time of merchant Inventory API request
        /// </summary>
        public int? Stock { get; set; }

       
    }
}
