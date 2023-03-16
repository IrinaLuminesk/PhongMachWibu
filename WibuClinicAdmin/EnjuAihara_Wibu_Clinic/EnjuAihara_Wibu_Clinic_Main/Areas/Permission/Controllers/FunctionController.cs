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
                ((x.FunctionId.Contains(search.FunctionId)) || (string.IsNullOrEmpty(search.FunctionId)) )&&
                ( (x.FunctionName.Contains(search.FunctionName)) || (string.IsNullOrEmpty(search.FunctionName)))
         
                ).ToList();
            return PartialView(kq);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(FunctionModel model)
        {
            //Báo lỗi
            JsonResult json = ValidateFunction(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                foreach (FunctionModel model2 in _context.FunctionModels)
                {
                    if (model2.FunctionId.Equals(model.FunctionId.ToUpper()))
                    {
                        return Json(new
                        {
                            isSucess = false,
                            title = "Lỗi",
                            message = "Mã chức năng đã tồn tại"
                        });
                    }
                }
                
                FunctionModel function = new FunctionModel()
                {
                    FunctionName = model.FunctionName,
                    FunctionId = model.FunctionId.ToUpper()
                };
                _context.FunctionModels.Add(function);
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Tạo Function mới thành công",
                    redirect = "/Permission/Function"
                });
            }catch(Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Tạo Function mới thất bại",
                    redirect = "/Permission/Function"
                });
            }
        }
        [HttpPost]
        public JsonResult Delete(string Id)
        {
            try
            {
                var pp = _context.PageFunctionModels.Where(x => x.FunctionId == Id).ToList();
                if (pp != null && pp.Count() > 0)
                {
                    _context.PageFunctionModels.RemoveRange(pp);
                }
                var pm = _context.PagePermissionModels.Where(x => x.FuntionId == Id).ToList();
                if (pm != null && pp.Count() > 0)
                {
                    _context.PagePermissionModels.RemoveRange(pm);
                }
                var f = _context.FunctionModels.Where(x => x.FunctionId == Id).FirstOrDefault();
                if (f != null)
                {
                    _context.FunctionModels.Remove(f);
                }
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Xóa chức năng thành công",
                    redirect = "/Permission/Function"
                }
                );
            }catch(Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = ex + " Xóa chức năng thất bại",
                    redirect = "/Permission/Function"
                });
            }

        }
        public JsonResult ValidateFunction(FunctionModel model)
        {
            if (string.IsNullOrEmpty(model.FunctionId))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống mã chức năng"
                });
            }
            if (string.IsNullOrEmpty(model.FunctionName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên chức năng"
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
        public JsonResult Edit(FunctionModel model)
        {
            //Báo lỗi
            JsonResult json = ValidateFunction(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                FunctionModel function = new FunctionModel()
                {
                    FunctionName = model.FunctionName,
                    FunctionId = model.FunctionId.ToUpper()
                };
                _context.Entry(function).State=EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Lưu Function thành công",
                    redirect = "/Permission/Function"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Lưu Function thất bại",
                    redirect = "/Permission/Function"
                });
            }
        }
        public void CreateViewBag()
        {
        }
    }
}