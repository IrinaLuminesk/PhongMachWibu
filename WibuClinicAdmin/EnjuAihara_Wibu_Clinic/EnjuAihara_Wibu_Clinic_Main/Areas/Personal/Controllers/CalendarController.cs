using EnjuAihara.Core;
using EnjuAihara.ViewModels.Personal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Personal.Controllers
{
    public class CalendarController : IrinaLumineskController
    {
        // GET: Personal/Calendar
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetPersonalEvent()
        {
            var Id = CurrentUser.AccountId;
            var list = _context.DateModels.Where(x => x.BacSiKham == Id && x.TrangThaiCuocHen == false && x.TrangThaiThanhToan == true && x.YTaXacNhan != null && x.AppointmentDate >= DateTime.Now)
                .Select(x => new CalendarEventViewModel
                {
                    id = x.DateCode,
                    title = "Khám cho " + x.AccountModel.UsersModel.FirstName + " " + x.AccountModel.UsersModel.LastName,
                    startDateTime = ((DateTime)x.AppointmentDate)
                }).ToList();
            foreach (var i in list)
            {
                i.start = i.startDateTime.ToString("yyyy-MM-dd");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}