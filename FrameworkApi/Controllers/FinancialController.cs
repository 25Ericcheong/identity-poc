using System.Web.Http;

namespace FrameworkApi.Controllers
{
    public class FinancialController : ApiController
    {
        [HttpGet]
        public string Index()
        {
            return "FINANCIAL DATA RECEIVED";
        }
    }
}