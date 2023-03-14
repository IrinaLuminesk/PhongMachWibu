using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EnjuAihara.EntityFramework;
using EnjuAihara.ViewModels;
using EnjuAihara.Utilities;
using EnjuAihara.Utilities.CloudinaryHelper;
using System.Security.Claims;
using System.Web.Routing;

namespace EnjuAihara.Core
{
    [Hado_Nejire_Authorization]
    public class IrinaLumineskController : System.Web.Mvc.Controller
    {
        public QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();

        protected IrinaLumineskController()
        {
            _context = new QuanLyPhongMachWibuEntities();
        }


        public AccountModel CurrentUser
        {
            get
            {
                ClaimsPrincipal currentUser = (ClaimsPrincipal)User;
                if (currentUser.Claims.Count() > 0)
                {
                    string username = currentUser.Claims.ElementAt(0).Value;
                    AccountModel ac = _context.AccountModels.Where(x => x.UserName.Equals(username)).FirstOrDefault();
                    if (ac == null)
                        return null;
                    return ac;
                }
                return null;
            }
        }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            SetPermissionSession();
        }


        public void SetPermissionSession()
        {
            if (Session["Permission"] == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (GetAllCurrentPermission() != null)
                    {
                        List<PagePermissionModel> Permissions = new List<PagePermissionModel>(GetAllCurrentPermission());
                        Session["Permission"] = Permissions;
                    }
                }
            }
            else
            {
                Session["Permission"] = null;
                if (GetAllCurrentPermission() != null)
                {
                    List<PagePermissionModel> Permissions = new List<PagePermissionModel>(GetAllCurrentPermission());
                    Session["Permission"] = Permissions;
                }
            }
        }

        public List<PagePermissionModel> GetAllCurrentPermission()
        {
            if (CurrentUser != null)
            {
                var AllRole = CurrentUser.AccountInRoleModels.Select(x => x.RoleId).ToList();
                List<PagePermissionModel> Permissions = new List<PagePermissionModel>();
                foreach (var i in AllRole)
                {
                    Permissions.AddRange(_context.PagePermissionModels.Where(x => x.RoleId == i).ToList());
                }
                return Permissions;
            }
            return null;
        }
    }
}
