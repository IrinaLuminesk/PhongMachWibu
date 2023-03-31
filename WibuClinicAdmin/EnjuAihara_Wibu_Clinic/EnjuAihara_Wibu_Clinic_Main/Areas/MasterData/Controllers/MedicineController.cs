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
            _context.Database.CommandTimeout = 100000;

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


                MedicineOnHandQuantity = x.WarehouseDetailModels.Select(y => y.InstockQuantity).Sum(),
                MaxPrice = x.WarehouseDetailModels.Max(y => y.SalePrice),
                Expiry = x.WarehouseDetailModels.OrderByDescending(y => y.ExpiredDate).Select(y => y.ExpiredDate).FirstOrDefault(),
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
                    var inStockQuantity = _context.WarehouseDetailModels.Where(x => x.MedicineProviderId == item.MedicineProvideId).Select(y => y.InstockQuantity).Sum();
                    if (inStockQuantity <= 0 || inStockQuantity == null)
                    {
                        if (inStockQuantity <= 0)
                        {
                            item.MaxPrice = _context.WarehouseDetailModels.Where(x => x.MedicineProviderId == item.MedicineProvideId).Max(y => y.BoughtPrice);
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
        public ActionResult Create(Guid? Id)
        {
            if (Id == null)
            {
                //CreateViewBag();
                return View();
            }
            else
            {
                var medicine = _context.MedicineModels.Where(x => x.MedicineId == Id).FirstOrDefault();
                return View(medicine);
            }
        }
        [HttpPost]
        public JsonResult Create(List<MedicineCreateViewModel> Med, string MedicineName, string Unit, Guid? MedicineId)
        {
            try
            {
                if(MedicineId != null)
                {
                    List<MedicineCreateViewModel> temp = new List<MedicineCreateViewModel>();
                    var temp2 = _context.MedicineProvideModels.Where(x => x.MedicineId == MedicineId && x.Actived == true).Select(x => x.ProviderId).ToList();
                    foreach (var i in temp2)
                    {
                        temp.Add(new MedicineCreateViewModel() { Img = null, Ingredient = null, Provider = (Guid)i });
                    }
                    temp.AddRange(Med);
                    JsonResult validateAddNCC = ValidateCreate(temp, "Temp", "Temp");
                    if (validateAddNCC != null)
                        return validateAddNCC;
                    if (Med != null && Med.Count() > 0)
                    {
                        Med.Skip(_context.MedicineProvideModels.Where(x => x.MedicineId == MedicineId).Count());
                        foreach (var i in Med)
                        {
                            if (i != null)
                            {
                                MedicineProvideModel provideModel = new MedicineProvideModel()
                                {
                                    Actived = true,
                                    MedicineProvideId = Guid.NewGuid(),
                                    MedicineId = MedicineId,
                                    ProviderId = i.Provider,
                                    ProductImage = i.Img == null ? "" : CloudinaryUpload.Upload(i.Img),
                                };
                                _context.Entry(provideModel).State = EntityState.Added;
                                _context.SaveChanges();
                                if (i.Ingredient != null && i.Ingredient.Count > 0)
                                {

                                    foreach (var j in i.Ingredient)
                                    {
                                        MedicineCompoundModel Ingredient = new MedicineCompoundModel()
                                        {
                                            Id = Guid.NewGuid(),
                                            IngredientId = j,
                                            MedicineId = provideModel.MedicineProvideId,
                                        };
                                        _context.Entry(Ingredient).State = EntityState.Added;
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    return Json(new
                    {
                        isSucess = true,
                        title = "Thành công",
                        message = "Thêm nhà sản xuất thành công"
                    });
                }
                JsonResult validate = ValidateCreate(Med, MedicineName, Unit);
                if (validate != null)
                    return validate;

                MedicineModel medicine = new MedicineModel()
                {
                    MedicineId = Guid.NewGuid(),
                    MedicineCode = DataCodeGenerate.ThuocCodeGen(),
                    MedicineName = MedicineName,
                    Unit = Unit
                };
                _context.Entry(medicine).State = EntityState.Added;
                _context.SaveChanges();
                foreach (var i in Med)
                {
                    if (i != null)
                    {
                        MedicineProvideModel provideModel = new MedicineProvideModel()
                        {
                            Actived = true,
                            MedicineProvideId = Guid.NewGuid(),
                            MedicineId = medicine.MedicineId,
                            ProviderId = i.Provider,
                            ProductImage = i.Img == null ? "" : CloudinaryUpload.Upload(i.Img),
                        };
                        _context.Entry(provideModel).State = EntityState.Added;
                        _context.SaveChanges();
                        if (i.Ingredient != null && i.Ingredient.Count > 0)
                        {

                            foreach (var j in i.Ingredient)
                            {
                                MedicineCompoundModel Ingredient = new MedicineCompoundModel()
                                {
                                    Id = Guid.NewGuid(),
                                    IngredientId = j,
                                    MedicineId = provideModel.MedicineProvideId,
                                };
                                _context.Entry(Ingredient).State = EntityState.Added;
                                _context.SaveChanges();
                            }
                        }
                    }
                }
                return Json(new
                {
                    isSucess = true,
                    title = "Tạo thuốc thành công",
                    message = "Tạo thuốc thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = string.Format("Đã có lỗi xảy ra: {0}", ex.Message.ToString())
                });
            }
        }
        public ActionResult Edit(Guid? Id)
        {
            //CreateViewBag();
            var thuoc = _context.MedicineProvideModels.Where(x => x.MedicineProvideId == Id).FirstOrDefault();
            return View(thuoc);
        }

        [HttpPost]
        public JsonResult Edit(MedicineEditViewModel Med)
        {
            try
            {
                List<MedicineCreateViewModel> temp = new List<MedicineCreateViewModel>();
                {
                    new MedicineCreateViewModel()
                    {
                        Ingredient = Med.Ingredient,
                        Img = Med.Img,
                        Provider = Med.Provider
                    };
                };
                JsonResult validate = ValidateCreate(temp, "Temp", "Temp");
                if (validate != null)
                    return validate;
                var EditModel = _context.MedicineProvideModels.Where(x => x.MedicineProvideId == Med.MedicineProvideId).FirstOrDefault();
                EditModel.Actived = Med.Actived;
                EditModel.ProviderId = Med.Provider;
                if (Med.Img != null)
                {
                    EditModel.ProductImage = CloudinaryUpload.Upload(Med.Img);
                }
                _context.Entry(EditModel).State = EntityState.Modified;
                var Ingredient = _context.MedicineCompoundModels.Where(x => x.MedicineId == EditModel.MedicineProvideId).ToList();
                _context.MedicineCompoundModels.RemoveRange(Ingredient);
                _context.SaveChanges();
                if (Med.Ingredient != null && Med.Ingredient.Count > 0)
                {
                    foreach (var i in Med.Ingredient)
                    {
                        MedicineCompoundModel Ing = new MedicineCompoundModel()
                        {
                            Id = Guid.NewGuid(),
                            MedicineId = EditModel.MedicineProvideId,
                            IngredientId = i
                        };
                        _context.Entry(Ing).State = EntityState.Added;
                        _context.SaveChanges();
                    }
                }
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Sửa thuốc thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = string.Format("Đã có lỗi xảy ra: {0}", ex.Message.ToString())
                });
            }
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
            result.AddRange(_context.ProviderModels.Where(x => (x.ProviderName.Contains(searchTerm) || x.ProviderCode
            .Contains(searchTerm)) && x.Actived == true ).Select(x =>
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
            result.AddRange(_context.IngredientModels.Where(x => 
            (x.IngredientCode.Contains(searchTerm) || x.IngredientName.Contains(searchTerm)) 
            && x.Actived == true)
                .Select(x =>
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
            result.AddRange(_context.ProviderModels.Where(x => (x.ProviderName.Contains(searchTerm) || x.ProviderCode
            .Contains(searchTerm)) && x.Actived == true).Select(x =>
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
            result.AddRange(_context.IngredientModels.Where(x => (x.IngredientCode.Contains(searchTerm) || x.IngredientName
            .Contains(searchTerm)) && x.Actived == true).Select(x =>
            new SelectListGuidForAutoComplete
            {
                value = x.IngredientId,
                text = x.IngredientCode + " | " + x.IngredientName
            }).Take(10).ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ValidateCreate(List<MedicineCreateViewModel> Med, string MedicineName, string Unit)
        {
            if (string.IsNullOrEmpty(MedicineName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng nhập tên thuốc"
                });
            }
            if (string.IsNullOrEmpty(Unit))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng nhập đơn vị tính của thuốc"
                });
            }
            var duplicate = Med.GroupBy(x => x.Provider).Where(g => g.Count() > 1).Select(y => new { Element = y.Key, Counter = y.Count() }).ToList();
            if (duplicate != null && duplicate.Count() > 0)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không chọn giá trị lập cho nhà cung cấp"
                });
            }
            int j = 1;
            foreach (var i in Med)
            {
                if (i.Provider == null || i.Provider.Equals(Guid.Empty))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = string.Format("Vui lòng không chọn nhà cung cấp cho thông tin chi tiết #{0}", j)
                    });
                }
                //if (i.Ingredient == null || i.Ingredient.Count <= 0)
                //{
                //    return Json(new
                //    {
                //        isSucess = false,
                //        title = "Lỗi",
                //        message = string.Format("Vui lòng không chọn ít nhất 1 thành phần cho thông tin chi tiết #{0}", j)
                //    });
                //}
                j++;
            }
            return null;
        }

        public JsonResult JqueryAutoCompleteMediName(string kq)
        {
            var MediLst = _context.MedicineModels.Where(x => x.MedicineName.Contains(kq)).Select(x => x.MedicineName).Take(10).ToList();
            return Json(MediLst, JsonRequestBehavior.AllowGet);
        }




        public ActionResult ViewMap(Guid Id)
        {
            ViewBag.MedicineId = Id;
            return View();
        }

        [HttpPost]
        public JsonResult GetMap(Guid Id)
        {
            var Coord = _context.MedicineProvideModels.Where(x => x.MedicineId == Id && ((x.ProviderModel.Latitude != null && x.ProviderModel.longitude != null) || !string.IsNullOrEmpty(x.ProviderModel.Address)))
            .Select(x =>
            new Coordinate()
            {
                Latitude = x.ProviderModel.Latitude,
                Longitude = x.ProviderModel.longitude,
                Address = x.ProviderModel.Address,
                Name = x.ProviderModel.ProviderName
            }).ToList();
            return Json(new { data = Coord, count = Coord.Count });
        }
    }
}