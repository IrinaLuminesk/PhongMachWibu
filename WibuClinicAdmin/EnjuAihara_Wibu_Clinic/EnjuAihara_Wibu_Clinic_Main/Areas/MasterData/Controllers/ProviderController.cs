using EnjuAihara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class ProviderController : IrinaLumineskController
    {
        // GET: MasterData/Provider
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(Guid id)
        {
            return View();
        }
        public void CreateViewBag()
        {

        }
    }
}