using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.Core;
using EnjuAihara.Utilities.Permission;
using EnjuAihara.ViewModels.Reports;

namespace EnjuAihara_Wibu_Clinic_Main.Controllers
{
    public class HomeController : IrinaLumineskController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }



        [HttpPost]
        public JsonResult GetTongTienTienChiTrongNam()
        {
            List<MonthMoneyViewModel> temp = GetAllMonth();
            var query = _context.Database.SqlQuery<MonthMoneyViewModel>("exec GetTongTienTrongNam").ToList();
            foreach (var i in query)
            {
                temp[i.Month - 1].Money += i.Money;
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetTongTienTienThuTrongNam()
        {
            List<MonthMoneyViewModel> temp = GetAllMonth();
            var query = _context.Database.SqlQuery<MonthMoneyViewModel>("exec TongTienThuTrongNam").ToList();
            foreach (var i in query)
            {
                temp[i.Month - 1].Money += i.Money;
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }


        public List<MonthMoneyViewModel> GetAllMonth()
        {
            List<MonthMoneyViewModel> temp = new List<MonthMoneyViewModel>();
            for (int i = 1; i <= 12; i++)
            {
                temp.Add(new MonthMoneyViewModel() { Month = i, Money = 0 });
            }
            return temp;
        }
        [HttpPost]
        public JsonResult WarehouseReport()
        {
            List<WarehouseReportViewModel> temp = GetAllMonthForWarehouseReport();
            var query = _context.Database.SqlQuery<WarehouseReportViewModel>("exec sp_GetBaoCaoNhapXuatThuoc").ToList();
            foreach (var i in query)
            {
                temp[i.ThangNhap - 1].SLNhap += i.SLNhap;
                temp[i.ThangNhap - 1].SLXuat += i.SLXuat;
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }
        public List<WarehouseReportViewModel> GetAllMonthForWarehouseReport()
        {
            List<WarehouseReportViewModel> temp = new List<WarehouseReportViewModel>();
            for (int i = 1; i <= 12; i++)
            {
                temp.Add(new WarehouseReportViewModel() { ThangNhap = i, SLNhap = 0,SLXuat=0 });
            }
            return temp;
        }

        [HttpPost]
        public JsonResult TopThuocSuDungNhieuNhatTrongNam()
        {
            var query = _context.Database.SqlQuery<TopThuocViewModel>("exec TopThuocSuDungNhieu").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }


    }
}