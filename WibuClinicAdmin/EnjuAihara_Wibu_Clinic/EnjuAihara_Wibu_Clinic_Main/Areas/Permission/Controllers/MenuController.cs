using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
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

            List<SelectBoolItem> StatusList = new List<SelectBoolItem>()
        {
            new SelectBoolItem() { id = true, name = "Đang sử dụng"},
            new SelectBoolItem() { id = false, name = "Ngừng sử dụng"}
        };
            ViewBag.Actived = new SelectList(StatusList, "id", "name");
        }


        public PartialViewResult _Search(MenuModel Search)
        {
            List<MenuModel> result = _context.MenuModels.ToList();
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
            return Json(new
            {
                isSucess = true,
                title = "Thành công",
                message = "Lưu Menu thành công",
                redirect = "/"
            });
        }

    }
}