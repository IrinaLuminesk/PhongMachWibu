using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    public class MenuController : IrinaLumineskController
    {
        // GET: Menu
        public ActionResult Index()
        {
            CreateViewBag();
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


        public ActionResult _Search(MenuModel Search)
        {
            var result = _context.MenuModels.ToList();
            return View("~/Areas/Permission/Menu/Views/_Search.cshtml",result);
        }
    }
}