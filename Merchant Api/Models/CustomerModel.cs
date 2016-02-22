using Merchant_Api.Models.Auth;

namespace Merchant_Api.Models
{
    /// <summary>
    /// Customer Data
    /// </summary>
    class CustomerModel : IUser
    {
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Email Address
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Shipping Address
        /// </summary>
        public AddressModel ShippingAddress { get; set; }
        /// <summary>
        /// Billing Address
        /// </summary>
        public AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Social user id
        /// </summary>
        public string UserId { get; set; }

        public bool IsAuthenticated { get; set; }

        public bool IsAuthorized { get; set; }

    }
}
