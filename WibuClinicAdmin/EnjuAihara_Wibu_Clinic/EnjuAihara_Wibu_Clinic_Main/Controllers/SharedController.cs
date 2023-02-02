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
            var result = _context.MenuModels.ToList();
            return PartialView("~/EnjuAihara_Wibu_Clinic_Main/Views/Shared/Sidebar.cshtml", result);
        }
    }
}