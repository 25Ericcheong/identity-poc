using System.Web.Http;
using FrameworkApi.RequestResponse;

namespace FrameworkApi.Controllers
{
    [AllowAnonymous]
    // [RequiredScope("financial-api")]
    [RoutePrefix("api/financial")]
    public class FinancialController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var data = new[] {
                new Financial { Code = "SOME_CODE", Value = 10000 },
                new Financial { Code = "OTHER_CODE", Value = 20000 },
            };
            
            var response = new FinancialResponse { Financials = data };
            return Json(response);
        }
    }
}