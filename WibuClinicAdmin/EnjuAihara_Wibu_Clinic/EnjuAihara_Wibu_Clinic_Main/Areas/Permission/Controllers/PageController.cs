using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.SelectListItemCustom;

using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using System.Web.Mvc;


namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class PageController : IrinaLumineskController
    {
        // GET: Permission/Page
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public PartialViewResult _Search(PageModel search)
        {
            var Result = _context.PageModels.Where(x =>
                                                    (x.PageName.Contains(search.PageName) || string.IsNullOrEmpty(search.PageName)) &&
                                                    (x.PageUrl.Contains(search.PageUrl) || string.IsNullOrEmpty(search.PageUrl)) &&
                                                    (x.MenuId == search.MenuId || search.MenuId == null) &&
                                                    (x.Actived == search.Actived || search.Actived == null)
                                                    ).ToList();
            return PartialView(Result);
        }

        public ActionResult Create()
        {
            CreateViewBag();
            CreateFunctionViewBag(null);
            return View();
        }
        [HttpPost]
        public JsonResult Create(PageModel model, List<string> Functionlst)
        {
            JsonResult json = ValidatePage(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                PageModel newPage = new PageModel()
                {
                    PageId = Guid.NewGuid(),
                    Actived = true,
                    Icon = model.Icon,
                    PageName = model.PageName,
                    OrderIndex = model.OrderIndex,
                    PageUrl = model.PageUrl,
                    MenuId = model.MenuId
                };
                _context.PageModels.Add(newPage);

                if (Functionlst.Count() > 0 && Functionlst != null)
                {
                    foreach (var i in Functionlst)
                    {
                        PageFunctionModel pageFunction = new PageFunctionModel()
                        {
                            PageFunctionId = Guid.NewGuid(),
                            FunctionId = i,
                            PageId = newPage.PageId
                        };
                        _context.PageFunctionModels.Add(pageFunction);
                        _context.SaveChanges();
                    }
                }
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Tạo trang mới thành công",
                    redirect = "/Permission/Page"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra" + Environment.NewLine + ex.Message,
                    redirect = "/Permission/Page"
                });
            }


        }


        public ActionResult Edit(Guid Id)
        {
            CreateViewBag();
            CreateFunctionViewBag(Id);
            var page = _context.PageModels.Where(x => x.PageId == Id).FirstOrDefault();
            return View(page);
        }

        [HttpPost]
        public JsonResult Edit(PageModel model, List<string> Functionlst)
        {
            JsonResult json = ValidatePage(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                PageModel modifiedPage = new PageModel()
                {
                    MenuId = model.MenuId,
                    Icon = model.Icon,
                    PageName = model.PageName,
                    OrderIndex = model.OrderIndex,
                    Actived = model.Actived,
                    PageUrl = model.PageUrl,
                    PageId = model.PageId
                };
                _context.Entry(modifiedPage).State = EntityState.Modified;
                _context.SaveChanges();

                //Lưu danh mục chức năng cho trang
                var functionlst = _context.PageFunctionModels.Where(x => x.PageId == model.PageId).ToList();
                //Xóa hết tạo lại
                if (functionlst.Count > 0)
                {
                    foreach (var i in functionlst)
                    {
                        _context.Entry(i).State = EntityState.Deleted;
                        _context.SaveChanges();
                    }
                }
                if (Functionlst == null)
                    Functionlst = new List<string>();
                foreach (var i in Functionlst)
                {
                    PageFunctionModel pagefunction = new PageFunctionModel()
                    {
                        PageFunctionId = Guid.NewGuid(),
                        FunctionId = i,
                        PageId = modifiedPage.PageId,
                    };
                    _context.Entry(pagefunction).State = EntityState.Added;
                    _context.SaveChanges();
                }    
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Chỉnh sửa trang " + model.PageName + " thành công",
                    redirect = "/Permission/Page"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra" + Environment.NewLine + ex.Message,
                    redirect = "/Permission/Page"
                });
            }
        }

        public JsonResult ValidatePage(PageModel model)
        {
            if (string.IsNullOrEmpty(model.PageName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên trang"
                });
            }
            if (string.IsNullOrEmpty(model.Icon))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống icon của trang"
                });
            }
            if (string.IsNullOrEmpty(model.PageUrl))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống đường dẫn của trang"
                });
            }
            if (model.MenuId == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn danh mục của trang"
                });
            }
            return null;
        }



        public void CreateViewBag()
        {
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");


            var MenuList = _context.MenuModels.Select(x =>
            new SelectGuidItem()
            {
                id = x.MenuId,
                name = x.MenuName
            }).ToList();

            ViewBag.MenuList = new SelectList(MenuList, "id", "name");
        }


        public void CreateFunctionViewBag(Guid? Id)
        {
            var FunctionLst = _context.FunctionModels.ToList();
            ViewBag.FunctionLst = FunctionLst;

            if (Id != null)
            {
                var SelectFunctionLst = _context.PageFunctionModels.Where(x => x.PageId == Id).Select(x => x.FunctionId).ToList();
                ViewBag.SelectFunctionLst = SelectFunctionLst;
            }
        }

    }
}