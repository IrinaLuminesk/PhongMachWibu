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
    }
}
