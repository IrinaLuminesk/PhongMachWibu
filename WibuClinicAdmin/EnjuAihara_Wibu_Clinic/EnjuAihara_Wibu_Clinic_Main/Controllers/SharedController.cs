using EnjuAihara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace EnjuAihara_Wibu_Clinic_Main.Controllers
{
    public class SharedController : IrinaLumineskController
    {
        // GET: Shared
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Sidebar()
        {
            var result = _context.MenuModels.Where(x => x.Actived == true).OrderByDescending(x => x.OrderIndex).ToList();
            return PartialView(result);
        }
    }
}