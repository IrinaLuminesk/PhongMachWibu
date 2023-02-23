using EnjuAihara.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
namespace EnjuAihara.Core
{
    public class Hado_Nejire_Authorization : ActionFilterAttribute
    {

        public static QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var functionLst = _context.FunctionModels.ToList();
            var Pagelst = _context.PageModels.ToList();
            string Action = filterContext.ActionDescriptor.ActionName;
            string Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var Area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;
            string AreaName = string.Empty;
            if (Area.ContainsKey("area"))
            {
                AreaName = Area["area"].ToString();
            }
            string PageUrl = string.Empty;
            if (!string.IsNullOrEmpty(AreaName))
            {
                PageUrl = string.Format("/{0}/{1}", AreaName, Controller);
            }
            else
            {
                PageUrl = string.Format("/{0}/{1}", Controller, Action);
            }
            if (functionLst.Any(x => x.FunctionId.Equals(Action.ToUpper())) && Pagelst.Any(x => x.PageUrl.Equals(PageUrl)))
            {
                var username = ClaimsPrincipal.Current.Identities.First().GetUserName();
                if (!string.IsNullOrEmpty(username))
                {
                    AccountModel ac = _context.AccountModels.Where(x => x.UserName.Equals(username)).FirstOrDefault();
                    foreach (var i in ac.AccountInRoleModels)
                    {
                        if (CheckAccessPermission(AreaName, Action, Controller, (Guid)i.RoleId) == true)
                        {
                            base.OnActionExecuting(filterContext);
                            return;
                        }
                    }
                    filterContext.Result = new RedirectResult("/Shared/Error");
                    return;
                    //bool flag = ac.AccountInRoleModels.Any(x => CheckAccessPermission(AreaName, Action, Controller, (Guid)x.RoleId));
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
            //if (userid != null && !string.IsNullOrEmpty(userid))
            //{

            //    base.OnActionExecuting(filterContext);

            //}
            //else
            //{
            //    //string redirectUrl = string.Format("?ReturnUrl={0}", filterContext.HttpContext.Request.Url.PathAndQuery);
            //    //filterContext.HttpContext.Response.Redirect(FormsAuthentication.LoginUrl + redirectUrl, true);
            //}
        }


        public static bool CheckAccessPermission(string Area, string Action, string Controller, Guid RoleId)
        {
            string PageUrl = string.Empty;
            if (!string.IsNullOrEmpty(Area))
            {
                PageUrl = string.Format("/{0}/{1}", Area, Controller);
            }
            else
            {
                PageUrl = string.Format("/{0}/{1}", Controller, Action);
            }
            var PageId = _context.PageModels.Where(x => x.PageUrl.Equals(PageUrl)).FirstOrDefault();

            var Permission = _context.PagePermissionModels.Where(x => x.RoleId == RoleId && x.FuntionId.Equals(Action.ToUpper().Trim()) && x.PageId == PageId.PageId).FirstOrDefault();
            if (Permission == null)
                return false;
            return true;
        }
    }
}
