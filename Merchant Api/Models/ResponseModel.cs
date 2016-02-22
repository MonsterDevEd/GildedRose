namespace Merchant_Api.Models
{
    /// <summary>
    /// Response data for api clients
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// Satus of reqest
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Helpful message to client
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Error Messasges if any
        /// </summary>
        public string ErrorMessage { get; set; }
    }


    /// <summary>
    /// Order Api response
    /// </summary>
    public class OrderResponse : ResponseModel
    {
        /// <summary>
        /// Sample transaction reference to successfully submitted order
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// Echo the order submitted to client
        /// </summary>
        public IOrder SubmittedOrder { get; set; }
    }

    /// <summary>
    /// API response status
    /// </summary>
    public enum StatusEnum
    {
        /// <summary>
        /// Successful api call
        /// </summary>
        Success,
        /// <summary>
        /// Unsuccessful api call
        /// </summary>
        Error,
        /// <summary>
        /// Successful api call with warning
        /// </summary>
        Warning
    }
}
