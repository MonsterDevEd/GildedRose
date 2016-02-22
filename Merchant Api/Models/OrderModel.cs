using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Merchant_Api.Models
{
    /// <summary>
    /// An order
    /// </summary>
    public class OrderModel: IOrder
    {
        /// <summary>
        /// Merchant Id
        /// </summary>
        [Required]
        public string MerchantId { get; set; }
        /// <summary>
        /// Ordered items
        /// </summary>
        [Required]
        public List<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Tax total if any
        /// </summary>
        public int TaxTotal { get; set; }

        /// <summary>
        /// Freight total if any
        /// </summary>
        public int ShippingTotal { get; set; }

        /// <summary>
        /// Discount if any
        /// </summary>
        public int DiscountTotal { get; set; }

        /// <summary>
        /// Client's order reference
        /// </summary>
        [Required]
        public string MerchantOrderReference { get; set; }

        /// <summary>
        /// Gilded Rose Order Id reference
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// Date in Epoch time
        /// </summary>
        [Required]
        public long? OrderDate { get; set; }

        /// <summary>
        /// Payload validation
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Subtotal
        /// </summary>
        /// <returns></returns>
        public int GetSubTotal()
        {
            var value = 0;
            if (OrderItems != null && OrderItems.Any())
            {
                OrderItems.ForEach(i => value += (i.GetExtendedPrice()));
            }
            return value;
        }

        /// <summary>
        /// Grand total of order
        /// </summary>
        /// <returns></returns>
        public int GetGrandTotal()
        {
            return (GetSubTotal() + TaxTotal + ShippingTotal) - DiscountTotal;
        }

       

    }

    /// <summary>
    /// An Order item
    /// </summary>
    public class OrderItem : Item, IItem
    {
        /// <summary>
        /// Qty ordered for item
        /// </summary>
        [Required]
        public int QtyOrdered { get; set; }

        /// <summary>
        /// Extended price of order line item
        /// </summary>
        /// <returns></returns>
        public int GetExtendedPrice()
        {
            return QtyOrdered * Price;
        }
    }

}
