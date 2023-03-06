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


        public JsonResult _PaggingServerSide(DatatableViewModel model, PrescriptionSearchViewModel search)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.DescriptionModels.
                Select(x =>
            new PrescriptionSearchViewModel
            {
                PrescriptionCode = x.DescriptionCode,
                PrescriptionId = x.DescriptionId,
                DoctorName = x.AccountModel.UsersModel.FirstName + " " + x.AccountModel.UsersModel.LastName,
                PatientName = x.CreateBy == null ? x.AnonymousClient : (x.AccountModel.UsersModel.FirstName + " " + x.AccountModel.UsersModel.LastName),
                MedicineList = x.DescriptionDetailModels.Select(z => ("Thuốc " + z.WarehouseModel.MedicineProvideModel.MedicineModel.MedicineName + " của " + z.WarehouseModel.MedicineProvideModel.ProviderModel.ProviderName)).ToList(),
                IllnessList = x.DescriptionIllnessModels.Select(z => z.IllnessModel.IllnessName).ToList(),
                Note = x.Note,
                TotalMoney = x.DescriptionDetailModels.Sum(z => (double)z.Quantity * z.WarehouseModel.SalePrice),
                Status = x.Payment == null ? "Chưa thanh toán" : "Đã thanh toán"
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