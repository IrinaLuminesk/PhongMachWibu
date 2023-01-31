using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission
{
    public class PermissionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Permission";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Permission",
                "Permission/{controller}/{action}/{id}",
                new { controller = "Permission", action = "Index", id = UrlParameter.Optional },
                new string[] { "Permission.Controllers" }
            );

            //context.MapRoute(
            //    "Permission_Account",
            //    "Permission/Account/{action}/{id}",
            //    new { controller = "Account", action = "Index", id = UrlParameter.Optional },
            //    new string[] { "Permission.Controllers" }
            //);
        }
    }
}