using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.CloudinaryHelper;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.Utilities.EncryptionAlgorithm;
using EnjuAihara.Utilities.RandomString;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

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
                Address=x.Address,
                Status=x.Actived==true? "Đang sử dụng":"Đã ngừng",
                

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
            try {
                ProviderModel newNcc = new ProviderModel()
                {
                    ProviderId = Guid.NewGuid(),
                    ProviderCode = DataCodeGenerate.ProviderCodeGen(),
                    Actived = true,
                    ProviderName = model.ProviderName,
                    
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
                //provider.ProviderCode = viewModel.ProviderCode;
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
        }
    }
}