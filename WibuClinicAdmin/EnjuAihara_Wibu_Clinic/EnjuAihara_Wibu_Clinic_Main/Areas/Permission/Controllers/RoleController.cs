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
            JsonResult json = ValidateRole(model);
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
        public JsonResult ValidateRole(RolesModel model)
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
        public ActionResult Edit(Guid id)
        {
            var role = _context.RolesModels.FirstOrDefault(x => x.RoleId.Equals(id));
            return View(role);
        }
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            try
            {
                var pageRole = _context.PagePermissionModels.Where(x => x.RoleId == id).ToList();
                if (pageRole != null && pageRole.Count() > 0)
                {
                    _context.PagePermissionModels.RemoveRange(pageRole);
                }
                var accountRole=_context.AccountInRoleModels.Where(x => x.RoleId == id).ToList();
                if(accountRole != null && accountRole.Count() > 0)
                {
                    _context.PagePermissionModels.RemoveRange(pageRole);
                }
                var role = _context.RolesModels.FirstOrDefault(x => x.RoleId == id);
                if (role != null)
                {
                    _context.RolesModels.Remove(role);

                }
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Xóa nhóm người dùng thành công",
                    redirect = "/Permission/Role"
                });
            }catch(Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = ex +" Xóa nhóm người dùng thất bại",
                    redirect = "/Permission/Role"
                });
            }
        }
        [HttpPost]
        public JsonResult Edit(RolesModel model)
        {
            //Báo lỗi
            JsonResult json = ValidateRole(model);
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
                    RoleId = model.RoleId,
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