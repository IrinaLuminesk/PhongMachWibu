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

namespace EnjuAihara.Core
{
    public class IrinaLumineskController : System.Web.Mvc.Controller
    {
        public QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();

        protected IrinaLumineskController()
        {
            _context = new QuanLyPhongMachWibuEntities();
        }
    }
}
