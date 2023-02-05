using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace EnjuAihara.Core
{
    public class Hado_Nejire_Authorization : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var userid = ClaimsPrincipal.Current.Identities.First().GetUserId();
            //if (userid != null && !string.IsNullOrEmpty(userid))
            //{

            //    base.OnActionExecuting(filterContext);

            //}
            //else
            //{
            //    //string redirectUrl = string.Format("?ReturnUrl={0}", filterContext.HttpContext.Request.Url.PathAndQuery);
            //    //filterContext.HttpContext.Response.Redirect(FormsAuthentication.LoginUrl + redirectUrl, true);
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}
