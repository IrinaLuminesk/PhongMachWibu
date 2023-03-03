using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class CityController : IrinaLumineskController
    {
        // GET: MasterData/City
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public void CreateViewBag()
        {

            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }


        public JsonResult _PaggingServerSide(DatatableViewModel model, CitySearchViewModel search, string CityName, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.CityModels.Where(x =>
            ( x.CityName.Contains(CityName) || string.IsNullOrEmpty(CityName)) &&
            (x.Actived == Actived || Actived == null)
            )
                .Select(x =>
            new CitySearchViewModel
            {
                CityId = x.CityId,
                CityName = x.CityName,
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngừng sử dụng"

            }).OrderBy(x => x.CityName).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<CitySearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
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


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(string CityName)
        {
            try
            {
                if (string.IsNullOrEmpty(CityName))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Tạo thất bại",
                        message = "Vui lòng không để trống tên đường"
                    });
                }
                CityModel create = new CityModel()
                {
                    CityName = CityName,
                    Actived = true,
                    CityId = Guid.NewGuid()
                };
                _context.Entry(create).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Tạo thành công",
                    message = string.Format("Tạo thành phố thành công")
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
            var City = _context.CityModels.Where(x => x.CityId == Id).FirstOrDefault();
            return View(City);
        }


        [HttpPost]
        public JsonResult Edit(string CityName, bool Actived, Guid CityId)
        {
            try
            {
                if (string.IsNullOrEmpty(CityName))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Tạo thất bại",
                        message = "Vui lòng không để trống tên đường"
                    });
                }
                var Edit = _context.CityModels.Where(x => x.CityId == CityId).FirstOrDefault();
                Edit.CityName = CityName;
                Edit.Actived = Actived;
                _context.Entry(Edit).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Sửa thành công",
                    message = string.Format("Sửa thành phố thành công")
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
        public JsonResult AutoCompleteCityName(string kq)
        {
            var CityLst = _context.CityModels.Where(x => x.CityName.Contains(kq) && x.Actived == true).OrderBy(x => x.CityName).Select(x => x.CityName).Take(10).ToList();
            return Json(CityLst, JsonRequestBehavior.AllowGet);
        }
    }
}