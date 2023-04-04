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
    public class IngredientController : IrinaLumineskController
    {
        // GET: MasterData/Ingredient
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public JsonResult _PaggingServerSide(DatatableViewModel model, IngredientSearchViewModel search, string IngredientCodeSearch, string IngredientNameSearch, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            //if (ToDate != null)
            //    ToDate = ((DateTime)ToDate).AddDays(1).AddSeconds(-1);

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.IngredientModels.
                Where(x => (x.IngredientCode.Contains(IngredientCodeSearch) || string.IsNullOrEmpty(IngredientCodeSearch))
                && (x.IngredientName.Contains(IngredientNameSearch) || string.IsNullOrEmpty(IngredientNameSearch))
         
                && (x.Actived == Actived || Actived == null)
                //&& (x.CreateDate >= FromDate || FromDate == null)
                //&& (x.CreateDate <= ToDate || ToDate == null)
                )
                .Select(x =>
            new IngredientSearchViewModel
            {
                IngredientId = x.IngredientId,
                IngredientCode = x.IngredientCode,
                IngredientName = x.IngredientName,
                //RoleName = x.RolesModels.Select(y => y.RoleName).ToList(),
                //RoleName = x.AccountInRoleModels.Select(y => y.RolesModel.RoleName).ToList(),
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngừng",


            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<IngredientSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
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
        public JsonResult Create(IngredientModel model)
        {
            if (string.IsNullOrEmpty(model.IngredientName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên thành phần thuốc"
                });
            }
            try
            {
                IngredientModel newtpt = new IngredientModel()
                {
                    IngredientId = Guid.NewGuid(),
                    IngredientCode = DataCodeGenerate.IngredientCodeGen(),
                    Actived = true,
                    IngredientName = model.IngredientName,

                };
                _context.Entry(newtpt).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Thêm thành phần thuốc thành công",
                    redirect = "/MasterData/Ingredient"
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
            var tp = _context.IngredientModels.FirstOrDefault(x => x.IngredientId == id);
            return View(tp);
        }
        [HttpPost]
        public JsonResult Edit(IngredientModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.IngredientName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên thành phần thuốc"
                });
            }
            if (viewModel.Actived == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống trạng thái thành phần thuốc"
                });
            }
            try
            {
                var ingre = _context.IngredientModels.FirstOrDefault(x => x.IngredientId == viewModel.IngredientId);
                ingre.IngredientName = viewModel.IngredientName;
                ingre.Actived = viewModel.Actived;
                //provider.ProviderCode = viewModel.ProviderCode;
                _context.Entry(ingre).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Sửa thành phần thuốc thành công",
                    redirect = "/MasterData/Ingredient"
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