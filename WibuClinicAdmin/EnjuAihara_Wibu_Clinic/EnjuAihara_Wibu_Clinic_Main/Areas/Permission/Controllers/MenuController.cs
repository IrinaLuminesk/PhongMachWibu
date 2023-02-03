using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
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
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }


        public PartialViewResult _Search(MenuModel Search)
        {
            List<MenuModel> result = _context.MenuModels.Where(x => (x.MenuName.Contains(Search.MenuName) || string.IsNullOrEmpty(Search.MenuName)) && (x.Actived == Search.Actived || Search.Actived == null)).ToList();
            return PartialView(result);
        }


        public ActionResult Create()
        {
            return View();
        }


        //Ghi chú cho Phước, nếu m có copy hàm lưu này của t thì đây là cách sử dụng.Nếu sau khi lưu thành công thì hã để cái isSucess = true
        //Nếu muốn có thêm thông báo sau khi lưu thành công thì để cái title là tiêu đề và message là tin nhắn thông báo lưu thành công
        //redirect là chuyển sang trang khác.
        //Nếu m muốn lưu thôi mà không hiện thông báo gì hết thì chỉ cần để isSucess = true
        [HttpPost]
        public JsonResult Create(MenuModel model)
        {
            JsonResult json = ValidateMenu(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                MenuModel newMenu = new MenuModel()
                {
                    MenuId = Guid.NewGuid(),
                    Actived = true,
                    Icon = model.Icon,
                    MenuName = model.MenuName,
                    OrderIndex = model.OrderIndex
                };
                _context.MenuModels.Add(newMenu);
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Tạo Menu mới thành công",
                    redirect = "/Permission/Menu"
                });

            } catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra" + Environment.NewLine + ex.Message,
                    redirect = "/Permission/Menu"
                });
            }
            

        }


        public JsonResult ValidateMenu(MenuModel model)
        {
            if (string.IsNullOrEmpty(model.MenuName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên Menu"
                });
            }
            if (string.IsNullOrEmpty(model.Icon))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống icon của Menu"
                });
            }
            return null;
        }


        public ActionResult Edit(Guid Id)
        {
            var menu = _context.MenuModels.Where(x => x.MenuId == Id).FirstOrDefault();
            return View(menu);
        }

        [HttpPost]
        public JsonResult Edit(MenuModel model)
        {
            JsonResult json = ValidateMenu(model);
            if (json != null)
            {
                return json;
            }
            try
            {
                MenuModel modifiedMenu = new MenuModel()
                {
                    MenuId = model.MenuId,
                    Icon = model.Icon,
                    MenuName = model.MenuName,
                    OrderIndex = model.OrderIndex,
                    Actived = model.Actived
                };
                _context.Entry(modifiedMenu).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Chỉnh sửa Menu " + model.MenuName + " mới thành công",
                    redirect = "/Permission/Menu"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra" + Environment.NewLine + ex.Message,
                    redirect = "/Permission/Menu"
                });
            }
        }

    }
}