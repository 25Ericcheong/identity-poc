using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace FrameworkApi
{
    public class RequireScopeAttribute: AuthorizeAttribute
    {
        private readonly string _scope;

        public RequireScopeAttribute(string scope)
        {
            _scope = scope;
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var user = actionContext.Request.GetOwinContext().Authentication.User;
            return user.Claims.Any(c => c.Type == ClaimTypes.Scope && c.Value == _scope);
        }

    }
}