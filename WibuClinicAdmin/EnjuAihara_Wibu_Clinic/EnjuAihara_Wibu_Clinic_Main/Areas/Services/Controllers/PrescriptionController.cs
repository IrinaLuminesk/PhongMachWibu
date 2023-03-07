using EnjuAihara.Core;
using EnjuAihara.ViewModels.Datatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.ViewModels.Services;
using EnjuAihara.Utilities.Datatable;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Services.Controllers
{
    public class PrescriptionController : IrinaLumineskController
    {
        // GET: Services/Prescription
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult _PaggingServerSide(DatatableViewModel model, PrescriptionSearchViewModel search, string PrescriptionCode, string Client, DateTime? FromDate, DateTime? Todate)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (Todate != null)
                Todate = ((DateTime)Todate).AddDays(1).AddSeconds(-1);


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.DescriptionModels.
                Where(x =>
                (x.DescriptionCode.Contains(PrescriptionCode) || string.IsNullOrEmpty(PrescriptionCode)) &&
                (x.AccountModel1.UsersModel.FirstName.Contains(Client) || x.AccountModel1.UsersModel.LastName.Contains(Client) || x.AnonymousClient.Contains(Client) || string.IsNullOrEmpty(Client)) &&
                (x.CreateDate >= FromDate || FromDate == null) &&
                (x.CreateDate <= Todate || Todate == null)
                ).
                Select(x =>
            new PrescriptionSearchViewModel
            {
                PrescriptionCode = x.DescriptionCode,
                PrescriptionId = x.DescriptionId,
                DoctorName = x.AccountModel.UsersModel.FirstName + " " + x.AccountModel.UsersModel.LastName,
                PatientName = x.CreateBy == null ? x.AnonymousClient : (x.AccountModel1.UsersModel.FirstName + " " + x.AccountModel1.UsersModel.LastName),
                MedicineList = x.DescriptionDetailModels.Select(z => ("Thuốc " + z.WarehouseModel.MedicineProvideModel.MedicineModel.MedicineName + " của " + z.WarehouseModel.MedicineProvideModel.ProviderModel.ProviderName)).ToList(),
                IllnessList = x.DescriptionIllnessModels.Select(z => z.IllnessModel.IllnessName).ToList(),
                Note = x.Note,
                TotalMoney = x.DescriptionDetailModels.Sum(z => (double)z.Quantity * z.WarehouseModel.SalePrice) + x.Payment == null ? 0 : x.Payment ,
                Status = x.IsPay == false ? "Chưa thanh toán" : "Đã thanh toán"
            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<PrescriptionSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
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
        public JsonResult _PaggingServerSideMedicine(DatatableViewModel model, MedicineSearchViewModel search, string MedList, string ProList)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var Med = _context.MedicineProvideModels.ToList();
            var Warehouse = _context.WarehouseModels.ToList();
            var query = Med.GroupJoin(Warehouse, m => m.MedicineProvideId, w => w.MedicineProviderId, (m, ws) =>
            new MedicineSearchViewModel
            {
                MedName = m.MedicineModel.MedicineName,
                ProName = m.ProviderModel.ProviderName,
                Price = m.WarehouseModels.
            });
            var finalResult = PaggingServerSideDatatable.DatatableSearch<PrescriptionSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ClientSearch(string kq)
        {
            List<string> ClientList = new List<string>();
            //Danh sách khách hàng có tài khoản
            var UserWithAccount = _context.AccountModels.Where(x => (x.UsersModel.FirstName.Contains(kq) || x.UsersModel.LastName.Contains(kq) || x.UserName.Contains(kq))).
                Select(x => (x.UsersModel.LastName + " " + x.UsersModel.FirstName)).Take(10).ToList();
            ClientList.AddRange(UserWithAccount);

            //Danh sách khách hàng không có tài khoản
            var AnonymousUser = _context.DescriptionModels.Where(x => x.AnonymousClient.Contains(kq)).Select(x => x.AnonymousClient).Distinct().Take(10).ToList();
            ClientList.AddRange(AnonymousUser);
            return Json(ClientList, JsonRequestBehavior.AllowGet);
        }
    }
}