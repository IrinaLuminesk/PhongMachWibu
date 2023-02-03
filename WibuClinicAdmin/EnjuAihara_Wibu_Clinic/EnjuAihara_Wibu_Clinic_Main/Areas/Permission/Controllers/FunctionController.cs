using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class FunctionController : IrinaLumineskController
    {
        // GET: Permission/Function
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public PartialViewResult _Search(FunctionModel search)
        {
            List<FunctionModel> kq = _context.FunctionModels.Where(x=>
                (x.FunctionId.Equals(search.FunctionId))
                || (x.FunctionName.Equals(search.FunctionName))
                ).ToList();
            return PartialView(kq);
        }
        public ActionResult Create()
        {
            return View();
        }
        public void CreateViewBag()
        {
            List<SelectBoolItem> StatusList = new List<SelectBoolItem>()
            {
                new SelectBoolItem() { id = true, name = "Đang sử dụng"},
                new SelectBoolItem() { id = false, name = "Ngừng sử dụng"}
            };
            ViewBag.Actived = new SelectList(StatusList, "id", "name");
        }
    }
}