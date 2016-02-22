using System.Threading.Tasks;
using System.Web.Mvc;
using Merchant_Api.Models;
using Merchant_Api.Models.Auth;
using Microsoft.Web.Mvc;

namespace Merchant_Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.GId = AuthUtils.GoogleClientId;
            return View();
        }

        [AjaxOnly, HttpPost]
        public async Task<JsonResult> BasicGoogleVerify(GLogin verify)
        {
            return Json(new GLogin
            {
                Token = verify.Token,
                Gid = verify.Gid,
                IsAuthenticated = await AuthUtils.VerifySocialTokenAsync(verify.Token, verify.Gid)
            });

        }


        
    }
}
