using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.ViewModels.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class ParametersConfigurationController : IrinaLumineskController
    {
        // GET: Permission/ParametersConfiguration
        public ActionResult Index()
        {
            var result = CreateConfig();
            return View(result);
        }


        public ConfigViewModel CreateConfig()
        {
            var result = _context.CatalogTypeModels.Where(x => x.CatalogTypeCode.Equals("TECHNICALCONFIGURATION")).FirstOrDefault();
            var result2 = _context.CatalogTypeModels.Where(x => x.CatalogTypeCode.Equals("VALUECONFIGURATION")).FirstOrDefault();
            var config = new ConfigViewModel()
            {
                InspectElement = GetInspectElementValue(result),
                DatCoc = GetTienDatCoc(result2),
                SoLuongBenhNhanBacSiXuLy = GetTongSoCuocHenBacSiXuLyTrongNgay(result2),
                SoLuongCuocHenTrongNgay = GetTongSoCuocHenTrongNgay(result2)
            };
            return config;
        }


        public bool GetInspectElementValue(CatalogTypeModel result)
        { 

            return Convert.ToBoolean(result.CatalogModels.Where(x => x.CatalogCode.Equals("InspectElement")).FirstOrDefault().Value);
        }

        public int GetTienDatCoc(CatalogTypeModel result)
        {

            return Convert.ToInt32(result.CatalogModels.Where(x => x.CatalogCode.Equals("PriceConfiguration")).FirstOrDefault().Value);
        }


        public int GetTongSoCuocHenTrongNgay(CatalogTypeModel result)
        {
            return Convert.ToInt32(result.CatalogModels.Where(x => x.CatalogCode.Equals("MaximumOccupancy")).FirstOrDefault().Value);
        }

        public int GetTongSoCuocHenBacSiXuLyTrongNgay(CatalogTypeModel result)
        {
            return Convert.ToInt32(result.CatalogModels.Where(x => x.CatalogCode.Equals("MaximumPatientOccupancyPerDoctor")).FirstOrDefault().Value);
        }

        [HttpPost]
        public JsonResult Edit(ConfigViewModel edit)
        {
            try
            {

                if (edit.DatCoc <= 0)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Thất bại",
                        message = "Vui lòng nhập số tiền cọc lớn hơn hoặc bằng 10.000đ"
                    });
                }

                if (edit.SoLuongBenhNhanBacSiXuLy <= 0)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Thất bại",
                        message = "Số lượng bệnh nhân được bác sĩ xử lý trong ngày phải lớn hơn 0"
                    });
                }

                if (edit.SoLuongCuocHenTrongNgay <= 0)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Thất bại",
                        message = "Số lượng cuộc hẹn trong ngày phải lớn hơn 0"
                    });
                }




                var InspectElement = _context.CatalogModels.Where(x => x.CatalogCode.Equals("InspectElement")).FirstOrDefault();
                InspectElement.Value = edit.InspectElement.ToString();
                _context.Entry(InspectElement).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();


                var TienDatCoc = _context.CatalogModels.Where(x => x.CatalogCode.Equals("PriceConfiguration")).FirstOrDefault();
                TienDatCoc.Value = edit.DatCoc.ToString();
                _context.Entry(TienDatCoc).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();


                var SoLuongBenhNhanToiDaTrongNgay = _context.CatalogModels.Where(x => x.CatalogCode.Equals("MaximumOccupancy")).FirstOrDefault();
                SoLuongBenhNhanToiDaTrongNgay.Value = edit.SoLuongCuocHenTrongNgay.ToString();
                _context.Entry(SoLuongBenhNhanToiDaTrongNgay).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

                var SoLuongBenhNhanToiDaCuaBacSiTrongNgay = _context.CatalogModels.Where(x => x.CatalogCode.Equals("MaximumPatientOccupancyPerDoctor")).FirstOrDefault();
                SoLuongBenhNhanToiDaCuaBacSiTrongNgay.Value = edit.SoLuongBenhNhanBacSiXuLy.ToString();
                _context.Entry(SoLuongBenhNhanToiDaCuaBacSiTrongNgay).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Thay đổi thông số kỹ thuật thành công"
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
    }
}