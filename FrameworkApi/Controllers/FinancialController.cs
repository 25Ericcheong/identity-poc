﻿using System.Web.Http;

namespace FrameworkApi.Controllers
{
    [Authorize]
    public class FinancialController : ApiController
    {
        [HttpGet]
        [RequireScope(Constants.AllowedScopes.FrameworkApiScope)]
        public string Index()
        {
            return $"FINANCIAL DATA COMING FROM NET48 API ({Constants.AllowedScopes.FrameworkApiScope} Protected)";
        }
    }
}