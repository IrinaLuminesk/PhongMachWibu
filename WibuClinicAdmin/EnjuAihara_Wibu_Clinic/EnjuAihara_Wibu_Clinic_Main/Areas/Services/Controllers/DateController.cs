using EnjuAihara.Core;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.ViewModels.SelectList;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Services.Controllers
{
    public class DateController : IrinaLumineskController
    {
        // GET: Services/Date
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult _PaggingServerSide(DatatableViewModel model, DateSearchViewModel search, DateTime? FromDate, DateTime? Todate)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (Todate != null)
                Todate = ((DateTime)Todate).AddDays(1).AddSeconds(-1);


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.DateModels.
                //Where(x =>
                //(x.DescriptionCode.Contains(PrescriptionCode) || string.IsNullOrEmpty(PrescriptionCode)) &&
                //(x.AccountModel1.UsersModel.FirstName.Contains(Client) || x.AccountModel1.UsersModel.LastName.Contains(Client) || x.AnonymousClient.Contains(Client) || string.IsNullOrEmpty(Client)) &&
                //(x.CreateDate >= FromDate || FromDate == null) &&
                //(x.CreateDate <= Todate || Todate == null)
                //).OrderBy(x => x.CreateDate).OrderBy(x => x.DescriptionCode).
                OrderBy(x => x.CreateDate).Select(x =>
            new DateSearchViewModel
            {
                DateCode = x.DateCode,
                DateCreate = x.CreateDate,
                Money = x.SoTienThanhToan,
                PayMentStatus = x.TrangThaiThanhToan == false ? "Chưa thanh toán" : "Đã thanh toán",
                DateId = x.DateId,
                Client = x.NguoiDatHen == null ? "" : x.AccountModel.UsersModel.LastName + " " + x.AccountModel.UsersModel.FirstName,
                Doctor = x.BacSiKham == null ? "" : x.AccountModel1.UsersModel.LastName + " " + x.AccountModel1.UsersModel.FirstName,
                Nurse = x.YTaXacNhan == null ? "" : x.AccountModel2.UsersModel.LastName + " " + x.AccountModel2.UsersModel.FirstName,
            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<DateSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.DateCreateString = FormatDateTime.FormatDateTimeWithString(item.DateCreate);
                }
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = finalResult
            });
        }


        public ActionResult AppointDoctor(Guid? Id)
        {
            var cuochen = _context.DateModels.Where(x => x.DateId == Id).FirstOrDefault();
            if (cuochen.TrangThaiCuocHen == true)
                return Redirect("/Shared/Error");
            return View(cuochen);
        }

        public JsonResult AutoCompleteDoctor(string searchTerm)
        {
            var result = _context.UsersModels.Where(x => (x.LastName.Contains(searchTerm) || x.FirstName.Contains(searchTerm)) && x.AccountModels.Any(y => y.AccountInRoleModels.Any(z => z.RolesModel.RoleName.Equals("Bác sĩ")))).Select(
                x => new SelectListGuidForAutoComplete
                {
                    value = x.AccountModels.Take(1).FirstOrDefault().AccountId,
                    text = x.LastName + " " + x.FirstName
                }
                ).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}