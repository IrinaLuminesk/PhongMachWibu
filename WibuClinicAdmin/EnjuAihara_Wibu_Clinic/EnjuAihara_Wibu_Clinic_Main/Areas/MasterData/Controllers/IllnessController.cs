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
    public class IllnessController : IrinaLumineskController
    {
        // GET: MasterData/Illness
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public void CreateViewBag()
        {

            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }

        public JsonResult _PaggingServerSide(DatatableViewModel model, IllnessSearchViewModel search, string IllnessName, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.IllnessModels.Where(x =>
            (x.IllnessName.Contains(IllnessName) || string.IsNullOrEmpty(IllnessName)) &&
            (x.Actived == Actived || Actived == null)
            )
            .Select(x =>
            new IllnessSearchViewModel
            {
               IllnessId = x.IllnessId,
               IllnessName = x.IllnessName,
               Status = x.Actived == true ? "Đang sử dụng" : "Đã ngưng sử dụng",
               TotalCase = x.DescriptionIllnessModels.Count()
            })

            .OrderByDescending(x => x.TotalCase).ToList();

            var finalResult = PaggingServerSideDatatable.DatatableSearch<IllnessSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
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
        public JsonResult Create(IllnessModel model)
        {
            try
            {
                JsonResult result = Validate(model);
                if (result != null)
                    return result;
                IllnessModel CreateModel = new IllnessModel()
                {
                    IllnessId = Guid.NewGuid(),
                    Actived = true,
                    IllnessName = model.IllnessName
                };
                _context.Entry(CreateModel).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = string.Format("Thêm thành công {0}", model.IllnessName),
                    redirect = "/MasterData/Illness"
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
            var Illness = _context.IllnessModels.Where(x => x.IllnessId == Id).FirstOrDefault();
            return View(Illness);
        }

        [HttpPost]
        public JsonResult Edit(IllnessModel model)
        {
            try
            {
                JsonResult result = Validate(model);
                if (result != null)
                    return result;
                var EditModel = _context.IllnessModels.Where(x => x.IllnessId == model.IllnessId).FirstOrDefault();
                EditModel.IllnessName = model.IllnessName;
                EditModel.Actived = model.Actived;
                _context.Entry(EditModel).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = string.Format("Sửa {0} thành công", model.IllnessName),
                    redirect = "/MasterData/Illness"
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

        public JsonResult Validate(IllnessModel model)
        {
            if (string.IsNullOrEmpty(model.IllnessName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên bệnh"
                });
            }
            return null;
        }

        [HttpPost]
        public JsonResult AutoComplete(string kq)
        {
            var IllnessLst = _context.IllnessModels.Where(x => x.IllnessName.Contains(kq)).Select(x => x.IllnessName).Take(10).ToList();
            return Json(IllnessLst, JsonRequestBehavior.AllowGet);
        }

    }
}