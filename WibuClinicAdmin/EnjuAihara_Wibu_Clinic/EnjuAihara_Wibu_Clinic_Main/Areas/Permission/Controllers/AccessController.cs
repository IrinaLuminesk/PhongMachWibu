using EnjuAihara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.ViewModels.SelectList;
using EnjuAihara.EntityFramework;
namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class AccessController : IrinaLumineskController
    {
        // GET: Permission/Access
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public void CreateViewBag()
        {
            var RoleList = _context.RolesModels.Where(x => !x.RoleName.Equals("Khách hàng")).Select(x =>
            new SelectGuidItem
            {
                id = x.RoleId,
                name = x.RoleName
            }).ToList();
            ViewBag.Roles = new SelectList(RoleList, "id", "name");
        }

        [HttpPost]
        public PartialViewResult GetPermissionPages(Guid RoleId)
        {
            var AllMenus = _context.MenuModels.ToList();
            ViewBag.RoleId = RoleId;
            return PartialView("Menus", AllMenus);
        }


        [ChildActionOnly]
        public PartialViewResult GetAllMenuPages(List<PageModel> pages, Guid RoleId)
        {
            ViewBag.RoleId = RoleId;
            return PartialView("Pages", pages);
        }

        [ChildActionOnly]
        public PartialViewResult GetAllPageFunction(Guid PageId, Guid RoleId)
        {
            var PageFunction = _context.PageFunctionModels.Where(x => x.PageId == PageId).OrderByDescending(x => x.FunctionId).ToList();
            var SelectPageFuntion = _context.PagePermissionModels.Where(x => x.PageId == PageId && x.RoleId == RoleId).ToList();
            ViewBag.SelectPageFuntion = SelectPageFuntion;
            ViewBag.PageId = PageId;
            ViewBag.RoleId = RoleId;
            return PartialView("PageFunction", PageFunction);
        }

        [HttpPost]
        public JsonResult Create(Guid PageId, Guid RoleId, string FunctionId)
        {
            try
            {
                PagePermissionModel model = new PagePermissionModel()
                {
                    PagePermissionId = Guid.NewGuid(),
                    FuntionId = FunctionId,
                    PageId = PageId,
                    RoleId = RoleId
                };
                _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false});
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid PageId, Guid RoleId, string FunctionId)
        {
            try
            {
                PagePermissionModel model = _context.PagePermissionModels.Where(x => x.PageId == PageId && x.RoleId == RoleId && x.FuntionId == FunctionId).FirstOrDefault();
                if (model != null)
                {
                    _context.Entry(model).State = System.Data.Entity.EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new { IsSuccess = true });
                }
                return Json(new { IsSuccess = false });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false });
            }
        }

    }
}