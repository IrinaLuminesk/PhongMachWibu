using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.ViewModels.SelectList;
using EnjuAihara.ViewModels.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.Utilities.CloudinaryHelper;
using EnjuAihara.Utilities.EncryptionAlgorithm;
using System.IO;
using Microsoft.AspNet.Identity;
using EnjuAihara.Utilities.RandomString;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Warehouse.Controllers
{
    public class StockReceivingController : IrinaLumineskController
    {
        // GET: Warehouse/StockReceiving
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public JsonResult _PaggingServerSide(DatatableViewModel model, StockReceivingSearchViewModel search, string ImportCodeSearch, Guid? MedicineIdSearch, Guid? ProviderIdSearch, DateTime? FromDate, DateTime? ToDate, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (ToDate != null)
                ToDate = ((DateTime)ToDate).AddDays(1).AddSeconds(-1);

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;
            
            var query = _context.WarehouseMasterModels.
                Where(x => (x.ImportCode.Contains(ImportCodeSearch) || string.IsNullOrEmpty(ImportCodeSearch))

                && (x.WarehouseDetailModels.Any(z=>z.MedicineProvideModel.MedicineId == MedicineIdSearch) || MedicineIdSearch == null||MedicineIdSearch==Guid.Empty)
                && (x.WarehouseDetailModels.Any(z => z.MedicineProvideModel.ProviderId == ProviderIdSearch) || ProviderIdSearch == null || ProviderIdSearch == Guid.Empty)
                // && (x.InstockQuantity == Actived || Actived == null)
                && (x.CreateDate >= FromDate || FromDate == null)
                && (x.CreateDate <= ToDate || ToDate == null)
                )
                .Select(x =>
            new StockReceivingSearchViewModel
            {
                ImportCode=x.ImportCode,
                WarehouseMasterId = x.WarehouseMasterId,
                WarehouseDetailId=x.WarehouseDetailModels.Select(y=>y.WarehouseDetailId).FirstOrDefault(),
                CreateDate=x.CreateDate,
                BoughtDate=x.BoughtDate,
                CreateBy=_context.AccountModels.Where(y=>y.AccountId==x.CreateBy).Select(y=>y.AccountModel2.UserName).FirstOrDefault()

            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<StockReceivingSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    //item.ExpiryString = FormatDateTime.FormatDateTimeWithString(item.ExpireDate);
                    item.BoughtDateString = FormatDateTime.FormatDateTimeWithString(item.BoughtDate);
                    item.CreateDateString= FormatDateTime.FormatDateTimeWithString(item.CreateDate);
                    item.STT = i;
                    //item.CreateDateString = FormatDateTime.FormatDateTimeWithString(item.CreateDate);
                    //item.LastLoginTimeString = FormatDateTime.FormatDateTimeWithString(item.LastLoginTime);
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
            CreateViewBag();
            ViewBag.CreateBy=CurrentUser.UserName;
            ViewBag.DateNow = DateTime.Now;
            return View();
        }
        public void CreateViewBag()
        {
            var medicine = _context.MedicineModels.Select(x =>
           new SelectGuidItem
           {
               id = x.MedicineId,
               name = x.MedicineName
           }).ToList();
            ViewBag.MedicineIdSearch = new SelectList(medicine, "id", "name");

            var provider = _context.ProviderModels.Select(x =>
          new SelectGuidItem
          {
              id = x.ProviderId,
              name = x.ProviderName
          }).ToList();
            ViewBag.ProviderIdSearch = new SelectList(provider, "id", "name");

            List<SelectBoolItem> StatusList = new List<SelectBoolItem>()
            {
                new SelectBoolItem() { id = true, name = "Còn tồn"},
                new SelectBoolItem() { id = false, name = "Đã hết trong kho"}
            };
            ViewBag.Actived = new SelectList(StatusList, "id", "name");
        }
    }
}