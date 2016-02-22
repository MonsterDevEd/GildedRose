namespace Merchant_Api.Models
{
    /// <summary>
    /// Addressing Information
    /// </summary>
    public class AddressModel
    {
        /// <summary>
        /// Street/Address Line 1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// Apt no, Ste /Address Line 2
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Zip Code
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

    }
}
