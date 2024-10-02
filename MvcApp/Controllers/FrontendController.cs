using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using MvcApp.RequestResponse;

namespace MvcApp.Controllers
{
    public class FrontendController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var authResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            var response = new FrontendResponse
            {
                IsAuthenticated = false,
            };

            if (authResult != null)
            {
                response.IsAuthenticated = authResult.Identity.IsAuthenticated;
            }
            
            var doc = new HtmlDocument();
            doc.Load(Server.MapPath("/front-end/dist/index.html"));

            var assets = new FrontEndAssets
            {
                BodyContent = doc.DocumentNode.SelectSingleNode("html/body")
                    .InnerHtml,
                HeadContent = doc.DocumentNode.SelectSingleNode("html/head")
                    .InnerHtml,
            };
            response.Assets = assets;
            
            return View("Index", response);
        }
        
    }
}