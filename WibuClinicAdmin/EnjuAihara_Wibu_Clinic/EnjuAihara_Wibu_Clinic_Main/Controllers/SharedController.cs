using EnjuAihara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Controllers
{
    public class SharedController : IrinaLumineskController
    {
        // GET: Shared
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sidebar()
        {
            var a = _context.MenuModels.ToList();
            return PartialView();
        }
    }
}