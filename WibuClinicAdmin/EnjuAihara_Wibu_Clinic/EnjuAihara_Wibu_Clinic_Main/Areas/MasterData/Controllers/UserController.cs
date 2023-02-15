using EnjuAihara.Core;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.DateTimeFormat;
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
    public class UserController : IrinaLumineskController
    {
        // GET: MasterData/User
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public JsonResult _PaggingServerSide(DatatableViewModel model, UserViewModel search)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.UsersModels.Select(x =>
            new UserViewModel
            {
                UserId = x.UserID,
                UserCodeResult = x.UserCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Birthday = x.Birthday,
                AddressResult = x.Address,
                PhoneResult = x.Phone,
                EmailResult = x.Email,
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngưng",
                AccountName = x.AccountModels.Select(y => y.UserName).ToList(),
                AccountId = x.AccountModels.Select(y => y.AccountId).ToList()
            }).OrderBy(x => x.UserCodeResult).ToList();

            var finalResult = PaggingServerSideDatatable.DatatableSearch<UserViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.BirthdayString = FormatDateTime.FormatDateTimeBirthday(item.Birthday);
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


        public ActionResult Edit()
        {
            return View();
        }
    }
}