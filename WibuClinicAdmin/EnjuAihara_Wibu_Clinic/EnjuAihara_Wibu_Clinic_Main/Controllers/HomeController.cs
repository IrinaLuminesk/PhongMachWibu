using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.Core;
using EnjuAihara.Utilities.Permission;
namespace EnjuAihara_Wibu_Clinic_Main.Controllers
{
    public class HomeController : IrinaLumineskController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}