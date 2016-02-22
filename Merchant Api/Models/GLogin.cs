using Merchant_Api.Models.Auth;

namespace Merchant_Api.Models
{
    public class GLogin : IUser
    {
        public bool IsAuthenticated { get; set; }
        public bool IsAuthorized { get; set; }
        public string Token { get; set; }
        public string Gid { get; set; }
    }
}
