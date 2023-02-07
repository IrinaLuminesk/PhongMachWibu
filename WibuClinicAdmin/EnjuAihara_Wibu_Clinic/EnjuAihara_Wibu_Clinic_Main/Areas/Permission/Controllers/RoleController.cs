using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.SelectListItemCustom;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class RoleController : IrinaLumineskController
    {
        // GET: Permission/Role
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public void CreateViewBag()
        {
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }
        public PartialViewResult _Search(RolesModel r)
        {
            List<RolesModel> roles = _context.RolesModels.Where(x=>
            (x.RoleCode.Contains(r.RoleCode)||string.IsNullOrEmpty(r.RoleCode))
            &&(x.RoleName.Contains(r.RoleName) || string.IsNullOrEmpty(r.RoleName))
            &&(x.Actived==r.Actived||r.Actived==null)
            ).ToList();
            return PartialView(roles);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(RolesModel model)
        {
            //Báo lỗi
            JsonResult json = ValidateFunction(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                foreach (RolesModel model2 in _context.RolesModels)
                {
                    if (model2.RoleCode.Equals(model.RoleCode.ToUpper()))
                    {
                        return Json(new
                        {
                            isSucess = false,
                            title = "Lỗi",
                            message = "Mã nhóm người dùng đã tồn tại"
                        });
                    }
                }

                RolesModel role = new RolesModel()
                {
                    RoleId = Guid.NewGuid(),
                    RoleName = model.RoleName,
                    RoleCode = model.RoleCode.ToUpper(),
                    Actived = true
                };
                _context.RolesModels.Add(role);
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Tạo nhóm người dùng mới thành công",
                    redirect = "/Permission/Role"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Tạo nhóm người dùng mới thất bại",
                    redirect = "/Permission/Role"
                });
            }
        }
        public JsonResult ValidateFunction(RolesModel model)
        {
            if (string.IsNullOrEmpty(model.RoleCode))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống mã nhóm người"
                });
            }
            if (string.IsNullOrEmpty(model.RoleName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên nhóm người"
                });
            }
            return null;
        }
        public ActionResult Edit(string id)
        {
            var function = _context.FunctionModels.FirstOrDefault(x => x.FunctionId.Equals(id));
            return View(function);
        }
        [HttpPost]
        public JsonResult Edit(RolesModel model)
        {
            //Báo lỗi
            JsonResult json = ValidateFunction(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                //foreach (RolesModel model2 in _context.RolesModels)
                //{
                //    if (model2.RoleCode.Equals(model.RoleCode.ToUpper()))
                //    {
                //        return Json(new
                //        {
                //            isSucess = false,
                //            title = "Lỗi",
                //            message = "Mã nhóm người dùng đã tồn tại"
                //        });
                //    }
                //}

                RolesModel role = new RolesModel()
                {
                    //RoleId = Guid.NewGuid(),
                    RoleName = model.RoleName,
                    RoleCode = model.RoleCode.ToUpper(),
                    Actived = model.Actived
                };
                _context.Entry(role).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Lưu nhóm người dùng thành công",
                    redirect = "/Permission/Role"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Lưu nhóm người dùng thất bại",
                    redirect = "/Permission/Role"
                });
            }
        }
    }
}