using EnjuAihara_Wibu_Clinic_Main.Areas.Report.XtraReport;
using EnjuAihara.Core;
using System;
using System.Web.Mvc;
using System.Data;
using System.Linq;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.ViewModels.Report;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Report.Controllers
{
    public class XuatNhapKhoController : IrinaLumineskController
    {
        // GET: Report/XuatNhapKho
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult _DocumentViewerPartial(DateTime FromDate, DateTime ToDate)
        {
            ReportXuatNhapKho a = new ReportXuatNhapKho();
            ViewBag.Report = CreateReport(FromDate, ToDate);
            return PartialView();
        }


        public ReportXuatNhapKho CreateReport(DateTime FromDate, DateTime ToDate)
        {
            ReportXuatNhapKho report = new ReportXuatNhapKho();
            report.DataSource = CreateData(FromDate, ToDate);
            return report;
        }

        public DataSet CreateData(DateTime FromDate, DateTime ToDate)
        {
            DataSet data = new DataSet();
            data.DataSetName = "TonKho";
            data.Tables.Add(new DataTable("Detail"));
            data.Tables.Add(new DataTable("HeaderDetail"));
            ToDate = ToDate.AddDays(1).AddSeconds(-1);
            var detail = _context.WarehouseModels.Where(x => x.BoughtDate >= FromDate && x.BoughtDate <= ToDate).Select(
                x => new XuatNhapKhoDetailViewModel
                {
                    MedicineCode = x.MedicineProvideModel.MedicineModel.MedicineCode,
                    MedicineName = x.MedicineProvideModel.MedicineModel.MedicineName,
                    Importdate = x.BoughtDate,
                    ImportTotal = x.BoughtQuantity,
                    Price = x.BoughtPrice,
                    Provider = x.MedicineProvideModel.ProviderModel.ProviderName,
                    Unit = x.MedicineProvideModel.MedicineModel.Unit
                }).OrderBy(x => x.Price).ToList();
            data.Tables["Detail"].Merge(ListtoDataTableConverter.ToDataTable<XuatNhapKhoDetailViewModel>(detail));
            return data;
        }
    }
}