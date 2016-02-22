namespace Merchant_Api.Models.Auth
{
    public interface IUser
    {
        bool IsAuthenticated { get; set; }
        bool IsAuthorized { get; set; }
    }
}
