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
using System.Net;
using System.Web.UI.WebControls;

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
                CreateBy=_context.AccountModels.Where(y=>y.AccountId==x.CreateBy).Select(y=>y.AccountModel2.UserName).FirstOrDefault(),
                Status=x.Actived==true ? "Đã duyệt":"Chưa được duyệt"

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
            ViewBag.CreateBy=CurrentUser.UserName;//Sử dụng được cái này nè mà ko sd đc UserId nó lấy đc nhưng nó lại không cho nhập
            ViewBag.DateNow = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.DateNow2 = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return View();
        }
        [HttpPost]
        public JsonResult Create(WarehouseMasterModel master,List<WarehouseDetailModel> stockReceivingDetailList)
        {
            try
            {


                var ngayNhap = ConvertDateTimeToInt(master.BoughtDate);
                var ngayTao = ConvertDateTimeToInt(master.CreateDate);
                if ((ngayNhap - ngayTao) > 7 || (ngayTao - ngayNhap) > 7)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Thất bại",
                        message = "Không được chọn ngày tương lai hoặc ngày quá khứ lớn hơn 1 tuần "
                    });
                }
                if (stockReceivingDetailList == null || stockReceivingDetailList.Count == 0)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Thất bại",
                        message = "Vui lòng chọn ít nhất 1 sản phẩm "
                    });
                }
                WarehouseMasterModel newMaster = new WarehouseMasterModel();
                newMaster.WarehouseMasterId = Guid.NewGuid();
                newMaster.ImportCode = DataCodeGenerate.WarehouseCodeGen();
                newMaster.BoughtDate = master.BoughtDate;
                newMaster.CreateDate = DateTime.Now;
                newMaster.CreateBy = CurrentUser.AccountId;
                _context.Entry(newMaster).State = EntityState.Added;
                _context.SaveChanges();
                if (stockReceivingDetailList.Count > 0 && stockReceivingDetailList != null)
                {
                    foreach (var detail in stockReceivingDetailList)
                    {
                        WarehouseDetailModel newDetail = new WarehouseDetailModel();
                        newDetail.WarehouseDetailId = Guid.NewGuid();
                        newDetail.MedicineProviderId = detail.MedicineProviderId;
                        newDetail.BoughtQuantity = detail.BoughtQuantity;
                        newDetail.ExpiredDate = detail.ExpiredDate;
                        newDetail.BoughtPrice = detail.BoughtPrice;
                        newDetail.InstockQuantity = detail.InstockQuantity;
                        newDetail.SalePrice = detail.SalePrice;
                        newDetail.SalePercentage = detail.SalePercentage;
                        newDetail.WarehouseMasterId = newMaster.WarehouseMasterId;
                        _context.Entry(newDetail).State = EntityState.Added;
                        _context.SaveChanges();

                    }
                }
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Nhập kho thành công",
                    redirect = "Index"
                });
            } catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = ex.InnerException.Message.ToString()
                });
            }
        }
        public ActionResult Detail(Guid? Id)
        {
            var master =_context.WarehouseMasterModels.Where(x => x.WarehouseMasterId == Id).FirstOrDefault();
            var detailList = _context.WarehouseDetailModels.Where(x => x.WarehouseMasterId == Id).Select(x => new StockReceivingDetailViewModel {
                WarehouseDetailId=x.WarehouseDetailId,
                WarehouseMasterId=x.WarehouseMasterId,
                MedicineProviderId=x.MedicineProviderId,
                MedicineCode=_context.MedicineModels.Where(z=>z.MedicineId==x.MedicineProvideModel.MedicineId).Select(z=>z.MedicineCode).FirstOrDefault(),
                MedicineName= _context.MedicineModels.Where(z => z.MedicineId == x.MedicineProvideModel.MedicineId).Select(z => z.MedicineName).FirstOrDefault(),
                BoughtQuantity=x.BoughtQuantity,
                BoughtPrice=x.BoughtPrice,
                SalePercentage=x.SalePercentage,
                SalePrice=x.SalePrice,
                ExpiredDate=x.ExpiredDate,
            }).ToList();
            ViewBag.ListStockRecevingDetail = detailList;
            return View(master);
        }
        [HttpPost]
        public JsonResult DuyetNhapKho(WarehouseMasterModel master)
        {
            var updateMaster=_context.WarehouseMasterModels.Where(x=>x.WarehouseMasterId==master.WarehouseMasterId).FirstOrDefault();
            updateMaster.Actived = true;
            _context.Entry(updateMaster).State = EntityState.Modified;
            _context.SaveChanges();
            return Json(new
            {
                isSucess = true,
                title = "Thành công",
                message = "Duyệt thành công",
                redirect = "/Warehouse/StockReceiving/Index"
            });
        }
        public int ConvertDateTimeToInt(DateTime? dateTime)
        {
            int dateKey;
            try
            {
                dateKey = Convert.ToInt32(string.Format("{0:yyyyMMdd}", dateTime));
                return dateKey;
            }
            catch
            {
                return 0;
            }
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

        [HttpPost]
        public ActionResult InsertProductStock(StockReceivingDetailViewModel model, List<StockReceivingDetailViewModel> stockReceivingDetailList)
        {

            JsonResult json = ValidateWd(model);
            if (json != null)
            {
                return json;
            }

            if (stockReceivingDetailList == null)
            {
                    stockReceivingDetailList = new List<StockReceivingDetailViewModel>();
            }
                var medicine = _context.MedicineModels.FirstOrDefault(p => p.MedicineId == model.MedicineId);
                if (medicine != null)
                {
                    model.MedicineCode = medicine.MedicineCode;
                    model.MedicineName = medicine.MedicineName;
                }

                stockReceivingDetailList.Add(model);
                return PartialView("_ProductStockDetailInner", stockReceivingDetailList);
                   
        }
        public ActionResult SearchMedicineByProvider(Guid ProviderId)
        {
            var thuoc = _context.MedicineProvideModels.Where(x => x.ProviderId == ProviderId).Select(x => new
            {
                Name=x.MedicineModel.MedicineName,
                Id=x.MedicineModel.MedicineId,
            }).ToList();
            return Json(thuoc, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveMedicineStock(List<StockReceivingDetailViewModel> stockReceivingDetailList, int STT)
        {
            stockReceivingDetailList = stockReceivingDetailList.Where(p => p.STT != STT).ToList();
            return PartialView("_ProductStockDetailInner", stockReceivingDetailList);
        }
        public ActionResult SearchMedicineProviderIdByProviderAndMedicine(Guid ProviderId,Guid MedicineId)
        {
            var thuocncc = _context.MedicineProvideModels.Where(x => x.ProviderId == ProviderId&& x.MedicineId==MedicineId).Select(x => x.MedicineProvideId).ToList();
            return Json(thuocncc, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInStockQuantity (Guid Medicine)
        {
            var sl = _context.WarehouseDetailModels.Where(x => x.MedicineProvideModel.MedicineId == Medicine&&x.ExpiredDate>=DateTime.Now).Sum(x=>x.InstockQuantity);
            return Json(sl, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateWd(StockReceivingDetailViewModel model)
        {
            if (model.MedicineId==null)
            {
                return Json("Vui lòng không để trống thuốc");
            }
            if (model.ProviderId==null)
            {
                return Json("Vui lòng không để trống nhà cung cấp");
            }
            if (model.InstockQuantity == null)
            {
                return Json("Vui lòng không để trống số lượng còn tồn");
            }
            if (model.BoughtQuantity == null)
            {
                return Json("Vui lòng không để trống số lượng nhập");
            }
            if (model.BoughtPrice == null)
            {
                return Json("Vui lòng không để trống giá mua vào");
            }
            if (model.SalePercentage == null)
            {
                return Json("Vui lòng không để trống chiết khấu");
            }
            if (model.SalePrice == null)
            {
                return Json("Vui lòng không để trống giá bán");
            }
            if (model.ExpiredDate == null)
            {
                return Json("Vui lòng không để trống hạn sử dụng");
            }
            return null;
        }
    }
}