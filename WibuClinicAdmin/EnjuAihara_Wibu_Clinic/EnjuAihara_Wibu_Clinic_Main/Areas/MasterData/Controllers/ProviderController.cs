using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.GoogleMap;
using EnjuAihara.Utilities.RandomString;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.ViewModels.SelectList;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class ProviderController : IrinaLumineskController
    {
        // GET: MasterData/Provider
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public JsonResult _PaggingServerSide(DatatableViewModel model, ProviderSearchViewModel search, string ProviderCodeSearch, string ProviderNameSearch,string AddressSearch , bool? Actived)
        { 
            int filteredResultsCount;
            int totalResultsCount = model.length;

            //if (ToDate != null)
            //    ToDate = ((DateTime)ToDate).AddDays(1).AddSeconds(-1);

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.ProviderModels.
                Where(x => (x.ProviderCode.Contains(ProviderCodeSearch) || string.IsNullOrEmpty(ProviderCodeSearch))
                && (x.ProviderName.Contains(ProviderNameSearch) || string.IsNullOrEmpty(ProviderNameSearch))
                && (x.Address.Contains(AddressSearch) || string.IsNullOrEmpty(AddressSearch))
                //&& (x.AccountInRoleModels.Any(z => z.RoleId == RoleNameSearch) || RoleNameSearch == null)
                && (x.Actived == Actived || Actived == null)
                //&& (x.CreateDate >= FromDate || FromDate == null)
                //&& (x.CreateDate <= ToDate || ToDate == null)
                )
                .Select(x =>
            new ProviderSearchViewModel
            {
                ProviderId = x.ProviderId,
                ProviderCode = x.ProviderCode,
                ProviderName = x.ProviderName,
                //RoleName = x.RolesModels.Select(y => y.RoleName).ToList(),
                //RoleName = x.AccountInRoleModels.Select(y => y.RolesModel.RoleName).ToList(),
                Address = x.Address  + (x.DistrictModel == null ? "" : " Quận " + x.DistrictModel.DistrictName) + (x.CityModel == null ? "" : " Thành phố " + x.CityModel.CityName),
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngừng",


            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<ProviderSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
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
            return View();
        }
        [HttpPost]
        public JsonResult Create(ProviderModel model)
        {
            if (string.IsNullOrEmpty(model.ProviderName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên nhà cung cấp"
                });
            }

            if (string.IsNullOrEmpty(model.Address))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống địa chỉ nhà cung cấp"
                });
            }
            //if (model.CityId == null)
            //{
            //    return Json(new
            //    {
            //        isSucess = false,
            //        title = "Lỗi",
            //        message = "Vui lòng không để trống thành phố của nhà cung cấp"
            //    });
            //}
            //if (model.DistrictId == null)
            //{
            //    return Json(new
            //    {
            //        isSucess = false,
            //        title = "Lỗi",
            //        message = "Vui lòng không để trống quận nhà cung cấp"
            //    });
            //}
            var Coordinate = GoogleMapUtilities.GetCoordinate(model.Address);
            if (Coordinate == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng nhập đúng địa chỉ thật"
                });
            }
            try {
                ProviderModel newNcc = new ProviderModel()
                {
                    ProviderId = Guid.NewGuid(),
                    ProviderCode = DataCodeGenerate.ProviderCodeGen(),
                    Actived = true,
                    ProviderName = model.ProviderName,
                    //CityId = model.CityId,
                    //DistrictId = model.DistrictId,
                    Latitude = Coordinate.Latitude,
                    longitude = Coordinate.Longitude,
                    Address = model.Address


                };
                _context.Entry(newNcc).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Thêm nhà cung cấp mới thành công",
                    redirect = "/MasterData/Provider"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra " + ex.Message.ToString()
                });
            }
        }
        public ActionResult Edit(Guid id)
        {
            var pro = _context.ProviderModels.FirstOrDefault(x => x.ProviderId == id);
            var CityList = _context.CityModels.Where(x => x.Actived == true).OrderBy(x => x.CityName).Select(x =>
            new SelectGuidItem
            {
                id = x.CityId,
                name = x.CityName
            }).ToList();

            ViewBag.CityList = new SelectList(CityList, "id", "name", pro.CityId);
            return View(pro);
        }
        [HttpPost]
        public JsonResult Edit(ProviderModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.ProviderName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên nhà cung cấp"
                });
            }
            if (string.IsNullOrEmpty(viewModel.Address))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống địa chỉ nhà cung cấp"
                });
            }
            //if (viewModel.CityId == null)
            //{
            //    return Json(new
            //    {
            //        isSucess = false,
            //        title = "Lỗi",
            //        message = "Vui lòng không để trống thành phố của nhà cung cấp"
            //    });
            //}
            //if (viewModel.DistrictId == null)
            //{
            //    return Json(new
            //    {
            //        isSucess = false,
            //        title = "Lỗi",
            //        message = "Vui lòng không để trống quận nhà cung cấp"
            //    });
            //}
            var Coordinate = GoogleMapUtilities.GetCoordinate(viewModel.Address);
            if (Coordinate == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng nhập đúng địa chỉ thật"
                });
            }
            if (viewModel.Actived==null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống trạng thái nhà cung cấp"
                });
            }
            try
            {
                var provider = _context.ProviderModels.FirstOrDefault(x => x.ProviderId == viewModel.ProviderId);
                provider.ProviderName = viewModel.ProviderName;
                provider.Actived = viewModel.Actived;
                //provider.CityId = viewModel.CityId;
                //provider.DistrictId = viewModel.DistrictId;
                provider.Address = viewModel.Address;
                provider.longitude = Coordinate.Longitude;
                provider.Latitude = Coordinate.Latitude;
                //provider.ProviderCode = viewModel.ProviderCode;
                _context.Entry(provider).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Sửa nhà cung cấp thành công",
                    redirect = "/MasterData/Provider"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra " + ex.Message.ToString()
                });
            }
        }
        public void CreateViewBag()
        {
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");

            var CityList = _context.CityModels.Where(x => x.Actived == true).OrderBy(x => x.CityName).Select(x =>
            new SelectGuidItemWithNull
            {
                id = x.CityId,
                name = x.CityName
            }).ToList();
            CityList.Add(new SelectGuidItemWithNull() { id = null, name = "-- Không chọn --" });
            ViewBag.CityList = new SelectList(CityList, "id", "name");
        }

        [HttpPost]
        public PartialViewResult GetDistrict(Guid Id)
        {
            var result = _context.DistrictModels.Where(x => x.CityId == Id).OrderBy(x => x.DistrictName).Select(x =>
            new SelectGuidItemWithNull
            {
                id = x.DistrictId,
                name = x.DistrictName
            }).ToList();
            result.Add(new SelectGuidItemWithNull() { id = null, name = "-- Không chọn --" });
            return PartialView(new SelectList(result, "id", "name"));
        }


        [HttpPost]
        public PartialViewResult GetMap()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase excelfile)
        {
            if (excelfile.ContentLength == 0)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Bạn chưa chọn file Excel!"
                });
            }
            else
            {
                if(excelfile.FileName.EndsWith("xls")|| excelfile.FileName.EndsWith("xlsx"))
                {
                    //Nhận file và lưu file vào thư mục Content
                    //string fileName = Path.GetFileName(excelfile.FileName);
                    string path = Server.MapPath("~/Content/"+ excelfile.FileName);
                    
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    excelfile.SaveAs(path);
                    //Giờ sẽ làm đọc file
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    //List<ProviderModel> lst = new List<ProviderModel>();
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        var Coordinate = GoogleMapUtilities.GetCoordinate(((Excel.Range)range.Cells[row, 2]).Text);
                        if (Coordinate == null)
                        {
                            return Json(new
                            {
                                isSucess = false,
                                title = "Lỗi",
                                message = "Vui lòng nhập đúng địa chỉ thật"
                            });
                        }
                    }
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        var Coordinate = GoogleMapUtilities.GetCoordinate(((Excel.Range)range.Cells[row, 2]).Text);
                        ProviderModel newNcc = new ProviderModel()
                        {
                            ProviderId = Guid.NewGuid(),
                            ProviderCode = DataCodeGenerate.ProviderCodeGen(),
                            Actived = true,
                            ProviderName = ((Excel.Range)range.Cells[row, 1]).Text,
                            Latitude = Coordinate.Latitude,
                            longitude = Coordinate.Longitude,
                            Address = ((Excel.Range)range.Cells[row, 2]).Text
                        };
                        _context.Entry(newNcc).State = EntityState.Added;
                        _context.SaveChanges();

                    }
                    return Json(new
                    {
                        isSucess = true,
                        title = "Thành công",
                        message = "Hệ thống đã nhận file thành công!"
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Đây không phải là file Excel!"
                    });
                }
            }
            
        }
    }
}