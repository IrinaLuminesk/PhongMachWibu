using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class StreetController : IrinaLumineskController
    {
        // GET: MasterData/Street
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }


        public ActionResult Create()
        {
            CreateViewBagCreate();
            return View();
        }

        [HttpPost]
        public JsonResult Create(StreetCreateViewModel model)
        {
            try
            {
                JsonResult result = Validate(model);
                if (result != null)
                    return result;
                StreetModel CreateModel = new StreetModel()
                {
                    Actived = true,
                    StreetId = Guid.NewGuid(),
                    StreetName = model.StreetName
                };
                _context.Entry(CreateModel).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                foreach (var i in model.DistrictId)
                {
                    DistrictStreetModel DistrictMoDel = new DistrictStreetModel()
                    {
                        DistrictStreetId = Guid.NewGuid(),
                        Actived = true,
                        DistrictId = i,
                        StreetId = CreateModel.StreetId
                    };
                    _context.Entry(DistrictMoDel).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    isSucess = true,
                    title = "Tạo thành công",
                    message = string.Format("Tạo đường thành công")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = string.Format("Đã có lỗi xảy ra {0}", ex.Message.ToString())
                });
            }
        }

        public JsonResult Validate(StreetCreateViewModel model)
        {
            if (string.IsNullOrEmpty(model.StreetName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên đường"
                });
            }
            if (model.CityId == null || model.CityId == Guid.Empty)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên thành phố"
                });
            }

            if (model.DistrictId == null || model.DistrictId.Count <= 0)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên quận huyện"
                });
            }
            return null;
        }

        public void CreateViewBagCreate()
        {
            ViewBag.CityList = new SelectList(_context.CityModels.Where(x => x.Actived == true).
                Select(x => new SelectGuidItem
                {
                    id = x.CityId,
                    name = x.CityName
                }), "id", "name");
        }

        public ActionResult Edit(Guid Id)
        {
            var Street = _context.DistrictStreetModels.Where(x => x.DistrictStreetId == Id).FirstOrDefault();
            ViewBag.CityList = new SelectList(_context.CityModels.Where(x => x.Actived == true).
                Select(x => new SelectGuidItem
                {
                    id = x.CityId,
                    name = x.CityName
                }), "id", "name", Street.DistrictModel.CityId);
            ViewBag.DistrictList = new SelectList(_context.DistrictModels.Where(x => x.Actived == true && x.CityId == Street.DistrictModel.CityId).
                Select(x => new SelectGuidItem
                {
                    id = x.DistrictId,
                    name = x.DistrictName
                }), "id", "name", Street.DistrictModel.DistrictId);
            return View(Street);
        }

        [HttpPost]
        public JsonResult Edit(Guid DistrictId, Guid CityId, bool Actived, Guid DistrictStreetId)
        {
            try
            {
                StreetCreateViewModel EditModel = new StreetCreateViewModel()
                {
                    CityId = CityId,
                    DistrictId = new List<Guid>() { DistrictId },
                    StreetName = "aa"
                };
                JsonResult result = Validate(EditModel);
                if (result != null)
                    return result;
                var Edit = _context.DistrictStreetModels.Where(x => x.DistrictStreetId == DistrictStreetId).FirstOrDefault();
                Edit.DistrictId = DistrictId;
                Edit.Actived = Actived;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Sửa thành công",
                    message = string.Format("Sửa đường thành công")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = string.Format("Đã có lỗi xảy ra {0}", ex.Message.ToString())
                });
            }
        }


        public void CreateViewBag()
        {

            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }

        public JsonResult _PaggingServerSide(DatatableViewModel model, StreetSearchViewModel search, string StreetName, string InDistrict, string InCity, bool? Actived)
        { 
            int filteredResultsCount;
            int totalResultsCount = model.length;

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.DistrictStreetModels.Where(x => 
            (x.StreetModel.StreetName.Contains(StreetName) || string.IsNullOrEmpty(StreetName)) &&
            (x.DistrictModel.DistrictName.Contains(InDistrict) || string.IsNullOrEmpty(InDistrict)) &&
            (x.DistrictModel.CityModel.CityName.Contains(InCity) || string.IsNullOrEmpty(InCity)) &&
            (x.Actived == Actived || Actived == null)
            )
                .Select(x =>
            new StreetSearchViewModel
            {
               StreetName = x.StreetModel.StreetName,
               InDistrict = x.DistrictModel.DistrictName,
               InCity = x.DistrictModel.CityModel.CityName,
               Status = x.Actived == true ? "Đang sử dụng" : "Đã ngừng sử dụng",
               StreetId = x.DistrictStreetId

            }).OrderBy(x => x.InCity).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<StreetSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
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
        public JsonResult AutoCompleteStreetName(string kq)
        {
            var IllnessLst = _context.StreetModels.Where(x => x.StreetName.Contains(kq) && x.Actived == true).Select(x => x.StreetName).Take(10).ToList();
            return Json(IllnessLst, JsonRequestBehavior.AllowGet);
        }

    }
}