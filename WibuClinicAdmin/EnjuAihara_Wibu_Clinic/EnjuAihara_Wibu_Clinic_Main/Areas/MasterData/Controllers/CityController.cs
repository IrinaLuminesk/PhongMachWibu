using EnjuAihara.Core;
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

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AutoCompleteCityName(string kq)
        {
            var CityLst = _context.CityModels.Where(x => x.CityName.Contains(kq) && x.Actived == true).OrderBy(x => x.CityName).Select(x => x.CityName).Take(10).ToList();
            return Json(CityLst, JsonRequestBehavior.AllowGet);
        }
    }
}