using EnjuAihara.Core;
using EnjuAihara.ViewModels.Datatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Services.Controllers
{
    public class PrescriptionController : IrinaLumineskController
    {
        // GET: Services/Prescription
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult _PaggingServerSide(DatatableViewModel model, MedicineSearchViewModel search, string MedicineNameSearch, string MedicineCodeSearch, Guid? ProviderNameSearch, Guid? IngredientNameSearch, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.MedicineProvideModels
                .Where(x => (x.MedicineModel.MedicineCode.Contains(MedicineCodeSearch) || string.IsNullOrEmpty(MedicineCodeSearch))
                && (x.MedicineModel.MedicineName.Contains(MedicineNameSearch) || string.IsNullOrEmpty(MedicineNameSearch))
                && (x.ProviderModel.ProviderId == ProviderNameSearch || ProviderNameSearch == Guid.Empty || ProviderNameSearch == null)
                && (x.MedicineCompoundModels.Any(y => y.IngredientModel.IngredientId == IngredientNameSearch) || IngredientNameSearch == null || IngredientNameSearch == Guid.Empty)
                && (x.Actived == Actived || Actived == null)
                ).Select(x =>
            new MedicineSearchViewModel
            {
                MedicineId = x.MedicineId,
                MedicineProvideId = x.MedicineProvideId,
                MedicineCode = x.MedicineModel.MedicineCode,
                MapId = x.MedicineId,
                MedicineName = x.MedicineModel.MedicineName,
                Unit = x.MedicineModel.Unit,
                IngredientName = _context.MedicineCompoundModels.Where(y => y.MedicineId == x.MedicineProvideId).Select(y => y.IngredientModel.IngredientName).ToList(),

                ProviderName = x.ProviderModel.ProviderName,


                MedicineOnHandQuantity = x.WarehouseModels.Select(y => y.InstockQuantity).Sum(),
                MaxPrice = x.WarehouseModels.Max(y => y.SalePrice),
                Expiry = x.WarehouseModels.OrderByDescending(y => y.ExpiredDate).Select(y => y.ExpiredDate).FirstOrDefault(),
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngưng"

            }).OrderBy(x => x.MedicineCode).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<MedicineSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.ExpiryString = FormatDateTime.FormatDateTimeWithString(item.Expiry);
                    var inStockQuantity = _context.WarehouseModels.Where(x => x.MedicineProviderId == item.MedicineProvideId).Select(y => y.InstockQuantity).Sum();
                    if (inStockQuantity <= 0 || inStockQuantity == null)
                    {
                        if (inStockQuantity <= 0)
                        {
                            item.MaxPrice = _context.WarehouseModels.Where(x => x.MedicineProviderId == item.MedicineProvideId).Max(y => y.BoughtPrice);
                        }
                        if (inStockQuantity == null)
                        {
                            item.MaxPrice = 0;
                        }
                        item.Status2 = "Đã hết trong kho";
                    }
                    else
                    {
                        item.Status2 = "Còn trong kho";
                    }
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