using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.ViewModels.SelectList;
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

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class MedicineController : IrinaLumineskController
    {
        // GET: MasterData/Medicine
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public JsonResult _PaggingServerSide(DatatableViewModel model,MedicineSearchViewModel  search, string MedicineNameSearch, string MedicineCodeSearch, Guid? ProviderNameSearch, Guid? IngredientNameSearch, bool? Actived)
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
                MedicineId = x.MedicineProvideId,
                MedicineName = x.MedicineModel.MedicineName,
                Unit = x.MedicineModel.Unit,
                IngredientName = _context.MedicineCompoundModels.Where(y => y.MedicineId == x.MedicineProvideId).Select(y => y.IngredientModel.IngredientName).ToList(),

                ProviderName = x.ProviderModel.ProviderName,


                MedicineOnHandQuantity = x.WarehouseModels.Select(y => y.InstockQuantity).Sum(),
                MaxPrice = x.WarehouseModels.Max(y => y.BoughtPrice),
                Expiry = x.WarehouseModels.OrderByDescending(y => y.ExpiredDate).Select(y => y.ExpiredDate).FirstOrDefault(),
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngưng"

            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<MedicineSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.ExpiryString = FormatDateTime.FormatDateTimeWithString(item.Expiry);
                    var inStockQuantity = _context.WarehouseModels.Where(x => x.MedicineProviderId == item.MedicineId).Select(y => y.InstockQuantity).Sum();
                    if (inStockQuantity <= 0 || inStockQuantity == null)
                    {
                        if (inStockQuantity <= 0)
                        {
                            item.MaxPrice = _context.WarehouseModels.Where(x => x.MedicineProviderId == item.MedicineId).Max(y => y.BoughtPrice);
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
        public void CreateViewBag()
        {
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
            var dsNcc = _context.ProviderModels.ToList();
            ViewBag.NccList = new SelectList(dsNcc, "ProviderId", "ProviderName");
            var dsThanhPhanList = _context.IngredientModels.ToList();
            ViewBag.ThanhPhanList = new SelectList(dsThanhPhanList, "IngredientId", "IngredientName");
        }
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        [HttpPost]
        public JsonResult Create(List<MedicineCreateViewModel> Med, string MedicineName, string Unit)
        {

            MedicineModel medicine = new MedicineModel()
            {
                MedicineId = Guid.NewGuid(),
                MedicineCode = DataCodeGenerate.ThuocCodeGen(),
                MedicineName = MedicineName,
                Unit = Unit
            };
            _context.Entry(medicine).State = EntityState.Added;

            //foreach (var i in Med)
            //{
            //    MedicineProvideModel detal = new MedicineProvideModel()
            //    {
            //        MedicineProvideId = Guid.NewGuid(),
            //        Actived = true,
            //        MedicineId = medicine.MedicineId,
            //        ProviderId = i.Provider,
            //        MedicineCompoundModels = new List<MedicineCompoundModel>()
            //        {
            //            new MedicineCompoundModel()
            //            {
                            
            //            }
            //        }
            //    }
            //}

            _context.SaveChanges();
            return Json(new
            {
                isSucess = false,
                title = "Đang thí nghiệm",
                message = "Tạo tài khoản mới thành công"
            });
        }
        public ActionResult Edit(Guid? id)
        {
            var thuoc = _context.MedicineModels.FirstOrDefault(x => x.MedicineId == id);
            return View(thuoc);
        }

        public PartialViewResult MedicineDetail(int index)
        {
            ViewBag.index = index;
            return PartialView();
        }

        public JsonResult AutoComplete(string searchTerm)
        {
            List<SelectListGuidForAutoComplete> result = new List<SelectListGuidForAutoComplete>()
            {
                new SelectListGuidForAutoComplete()
                {
                    text = "--Chọn tất cả--",
                    value = Guid.Empty
                }
            };
            result.AddRange(_context.ProviderModels.Where(x => x.ProviderName.Contains(searchTerm) || x.ProviderCode
            .Contains(searchTerm) ).Select(x =>
            new SelectListGuidForAutoComplete
            {
                value = x.ProviderId,
                text = x.ProviderCode + " | " + x.ProviderName
            }).Take(10).ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoComplete2(string searchTerm)
        {
            List<SelectListGuidForAutoComplete> result = new List<SelectListGuidForAutoComplete>()
            {
                new SelectListGuidForAutoComplete()
                {
                    text = "--Chọn tất cả--",
                    value = Guid.Empty
                }
            };
            result.AddRange(_context.IngredientModels.Where(x => x.IngredientCode.Contains(searchTerm) || x.IngredientName
            .Contains(searchTerm)).Select(x =>
            new SelectListGuidForAutoComplete
            {
                value = x.IngredientId,
                text = x.IngredientCode + " | " + x.IngredientName
            }).Take(10).ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult AutoCompleteNotAll(string searchTerm)
        {
            List<SelectListGuidForAutoComplete> result = new List<SelectListGuidForAutoComplete>();
            result.AddRange(_context.ProviderModels.Where(x => x.ProviderName.Contains(searchTerm) || x.ProviderCode
            .Contains(searchTerm)).Select(x =>
           new SelectListGuidForAutoComplete
           {
               value = x.ProviderId,
               text = x.ProviderCode + " | " + x.ProviderName
           }).Take(10).ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoComplete2NotAll(string searchTerm)
        {
            List<SelectListGuidForAutoComplete> result = new List<SelectListGuidForAutoComplete>();
            result.AddRange(_context.IngredientModels.Where(x => x.IngredientCode.Contains(searchTerm) || x.IngredientName
            .Contains(searchTerm)).Select(x =>
            new SelectListGuidForAutoComplete
            {
                value = x.IngredientId,
                text = x.IngredientCode + " | " + x.IngredientName
            }).Take(10).ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }









        public ActionResult ViewMap(Guid Id)
        {
            ViewBag.MedicineId = Id;    
            return View();
        }


        public JsonResult GetCoord(Guid Id)
        {
            var medicine = _context.MedicineModels.Where(x => x.MedicineId == Id).FirstOrDefault();
            List<MedicineProvideModel> Pro = medicine.MedicineProvideModels.ToList();
            List<Coordinate> coords = new List<Coordinate>();
            foreach (var i in Pro)
            {
                if (i.ProviderModel.Latitude != null && i.ProviderModel.longitude != null)
                {
                    coords.Add(new Coordinate()
                    {
                        Latitude = i.ProviderModel.Latitude,
                        Longitude = i.ProviderModel.longitude
                    });
                }
            }
            return Json(coords);
        }
    }
}