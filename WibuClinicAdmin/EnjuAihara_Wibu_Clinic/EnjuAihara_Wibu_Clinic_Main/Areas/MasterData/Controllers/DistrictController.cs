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
    public class DistrictController : IrinaLumineskController
    {
        // GET: MasterData/District
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }


        public JsonResult _PaggingServerSide(DatatableViewModel model, DistrictSearchViewModel search, string DistrictName, string InCity, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.DistrictModels.Where(x =>
            (x.DistrictName.Contains(DistrictName) || string.IsNullOrEmpty(DistrictName)) &&
             (x.CityModel.CityName.Contains(InCity) || string.IsNullOrEmpty(InCity)) &&
            (x.Actived == Actived || Actived == null)
            )
                .Select(x =>
            new DistrictSearchViewModel
            {
                DistrictId = x.DistrictId,
                InCity = x.CityModel.CityName,
                DistrictName = x.DistrictName,
                TotalRoad = x.DistrictStreetModels.Count(),
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngừng sử dụng"

            }).OrderBy(x => x.DistrictName).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<DistrictSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
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



        public void CreateViewBag()
        {

            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }

        public ActionResult Create()
        {
            ViewBag.CityList = new SelectList(_context.CityModels.Where(x => x.Actived == true).Select(x => new SelectGuidItem()
            {

                id = x.CityId,
                name = x.CityName
            }).ToList(), "id", "name");
            return View();
        }

        [HttpPost]
        public JsonResult Create(string DistrictName, Guid CityName)
        {
            try
            {
                if (string.IsNullOrEmpty(DistrictName))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không để trống tên quận"
                    });
                }
                if (CityName == null ||CityName == Guid.Empty)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng chọn thành phố"
                    });
                }
                DistrictModel create = new DistrictModel()
                {
                    DistrictId = Guid.NewGuid(),
                    Actived = true,
                    CityId = CityName,
                    DistrictName = DistrictName
                };
                _context.Entry(create).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Tạo thành công",
                    message = "Tạo quận thành công"
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

        public ActionResult Edit(Guid Id)
        {
            var district = _context.DistrictModels.Where(x => x.DistrictId == Id).FirstOrDefault();
            ViewBag.CityList = new SelectList(_context.CityModels.Where(x => x.Actived == true).Select(x => new SelectGuidItem()
            {

                id = x.CityId,
                name = x.CityName
            }).ToList(), "id", "name", district.CityId );
            return View(district);
        }

        [HttpPost]
        public JsonResult Edit(string DistrictName, Guid CityName, bool Actived, Guid DistrictId)
        {
            try
            {
                if (CityName == null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Sửa thất bại",
                        message = "Vui lòng không để trống tên thành phố"
                    });
                }
                if (string.IsNullOrEmpty(DistrictName))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Tạo thất bại",
                        message = "Vui lòng không để trống tên quận"
                    });
                }
                var Edit = _context.DistrictModels.Where(x => x.DistrictId == DistrictId).FirstOrDefault();
                Edit.DistrictName = DistrictName;
                Edit.CityId = CityName;
                Edit.Actived = Actived;
                _context.Entry(Edit).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Sửa thành công",
                    message = string.Format("Sửa quận thành công")
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

        [HttpPost]
        public JsonResult AutoCompleteDistrictName(string kq)
        {
            var DistrictLst = _context.DistrictModels.Where(x => x.DistrictName.Contains(kq) && x.Actived == true).OrderBy(x => x.DistrictName).Select(x => x.DistrictName).Take(10).ToList();
            return Json(DistrictLst, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult GetDistrictByCity(Guid CityId, bool? Multiple)
        {
            ViewBag.Multiple = Multiple;
            ViewBag.DistrictList = new SelectList(_context.DistrictModels.Where(x => x.CityId == CityId && x.Actived == true).
                Select(x => new SelectGuidItem
                {
                    id = x.DistrictId,
                    name = x.DistrictName
                }).ToList(), "id", "name");
            return PartialView();
        }
    }
}