using EnjuAihara.Core;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.Services;
using System;
using System.Linq;
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
                TrangThaiCuocHen = x.TrangThaiCuocHen == false ? "Chưa khám" : "Đã khám",
                PatientId = x.NguoiDatHen

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


        [HttpPost]
        public JsonResult ConfirmFinish(Guid Id)
        {
            try
            {
                var result = _context.DateModels.Where(x => x.DateId == Id).FirstOrDefault();
                result.TrangThaiCuocHen = true;
                _context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Xác nhận hoàn thành thành công",
                    redirect = "/Services/Date"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = string.Format("Lỗi: {0}", ex.Message.ToString()),
                    redirect = "/Services/Date"
                });
            }
        }

        public ActionResult AppointDoctor(Guid? Id)
        {
            var cuochen = _context.DateModels.Where(x => x.DateId == Id).FirstOrDefault();
            if (cuochen.TrangThaiCuocHen == true)
                return Redirect("/Shared/Error");
            return View(cuochen);
        }


        [HttpPost]
        public JsonResult AppointDoctor(Guid DateId, Guid DoctorId)
        {
            try
            {
                int SoBenhNhanDuocKhamCuaBacSi = Convert.ToInt32(_context.CatalogModels.Where(x => x.CatalogCode.Equals("MaximumPatientOccupancyPerDoctor")).FirstOrDefault().Value);
                var TuNgayMoi = DateTime.Now.AddDays(-1).AddSeconds(1);
                var DenNgayMoi = DateTime.Now.AddDays(1).AddSeconds(-1);
                var SoBenhNhanVangLai = _context.DescriptionModels.Where(x => x.CreateDate >= TuNgayMoi && x.CreateDate <= DenNgayMoi && x.CreateBy == DoctorId && !string.IsNullOrEmpty(x.AnonymousClient) && !string.IsNullOrEmpty(x.AnonymousPhone)).ToList().Count();
                var SoBenhNhanDaXuLy = _context.DateModels.Where(x => x.CreateDate >= TuNgayMoi && x.CreateDate <= DenNgayMoi && x.BacSiKham == DoctorId).ToList().Count();
                if ((SoBenhNhanVangLai + SoBenhNhanDaXuLy) > SoBenhNhanDuocKhamCuaBacSi)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Thất bại",
                        message = "Bác sĩ đã vượt quá số bệnh nhân quy định được khám"
                    });
                }
                var date = _context.DateModels.Where(x => x.DateId == DateId).FirstOrDefault();
                date.BacSiKham = DoctorId;
                date.YTaXacNhan = CurrentUser.AccountId;
                _context.Entry(date).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Chỉ định bác sĩ khám thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Lỗi " + ex.Message.ToString()
                });
            }
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