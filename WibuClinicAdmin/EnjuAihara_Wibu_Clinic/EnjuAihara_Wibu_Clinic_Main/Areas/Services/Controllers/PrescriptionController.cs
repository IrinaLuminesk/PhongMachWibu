using EnjuAihara.Core;
using EnjuAihara.ViewModels.Datatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.ViewModels.Services;
using EnjuAihara.Utilities.Datatable;
using System.Data.SqlClient;
using EnjuAihara.ViewModels.SelectList;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.RandomString;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.Utilities.LinqExtension;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Services.Controllers
{
    public class PrescriptionController : IrinaLumineskController
    {
        // GET: Services/Prescription
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult _PaggingServerSide(DatatableViewModel model, PrescriptionSearchViewModel search, string PrescriptionCode, string Client, DateTime? FromDate, DateTime? Todate)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (Todate != null)
                Todate = ((DateTime)Todate).AddDays(1).AddSeconds(-1);


            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.DescriptionModels.
                Where(x =>
                (x.DescriptionCode.Contains(PrescriptionCode) || string.IsNullOrEmpty(PrescriptionCode)) &&
                (x.AccountModel1.UsersModel.FirstName.Contains(Client) || x.AccountModel1.UsersModel.LastName.Contains(Client) || x.AnonymousClient.Contains(Client) || string.IsNullOrEmpty(Client)) &&
                (x.CreateDate >= FromDate || FromDate == null) &&
                (x.CreateDate <= Todate || Todate == null)
                ).
                Select(x =>
            new PrescriptionSearchViewModel
            {
                PrescriptionCode = x.DescriptionCode,
                PrescriptionId = x.DescriptionId,
                DoctorName = x.AccountModel.UsersModel.FirstName + " " + x.AccountModel.UsersModel.LastName,
                PatientName = x.CreateFor == null ? x.AnonymousClient : (x.AccountModel1.UsersModel.FirstName + " " + x.AccountModel1.UsersModel.LastName),
                MedicineList = x.DescriptionDetailModels.Select(z => ("Thuốc " + z.WarehouseDetailModel.MedicineProvideModel.MedicineModel.MedicineName + " của " + z.WarehouseDetailModel.MedicineProvideModel.ProviderModel.ProviderName)).ToList(),
                IllnessList = x.DescriptionIllnessModels.Select(z => z.IllnessModel.IllnessName).ToList(),
                Note = x.Note,
                TotalMoney = x.DescriptionDetailModels.Sum(z => (double)z.Quantity * z.WarehouseDetailModel.SalePrice) + x.Payment == null ? 0 : x.Payment ,
                Status = x.IsPay == false ? "Chưa thanh toán" : "Đã thanh toán",
                IsPay = x.IsPay
            }).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<PrescriptionSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
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
        public JsonResult _PaggingServerSideMedicine(DatatableViewModel model, MedicineSearchViewModel search, string MedList, string ProList)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;
            List<SqlParameter> param = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@MedList",
                    Value = MedList
                },
                new SqlParameter()
                {
                    ParameterName = "@ProList",
                    Value = ProList
                }
            };
            var query = _context.Database.SqlQuery<MedicineSearchViewModel>("exec GetMedicieInWarehouse @MedList, @ProList", param.ToArray()).ToList();
            //var Tempquery = query.Distinct(x ).Where(x => x.SoLuongTon == null || x.SoLuongTon <= 0)
            //var Tempquery = LinqExtension.DistinctBy(query.Where(x => x.SoLuongTon == null || x.SoLuongTon <= 0).ToList(), x => x.MedicineProviderId).ToList();
            //query.RemoveAll(x => x.SoLuongTon == null || x.SoLuongTon <= 0);
            //query.AddRange(Tempquery);
            var finalResult = PaggingServerSideDatatable.DatatableSearch<MedicineSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.HanSuDungString = FormatDateTime.FormatDateTimeWithString(item.HanSuDung);
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
        public JsonResult Create(PrescriptionCreateViewModel model, List<PrescriptionDetailCreateViewModel> Prescription)
        {
            try
            {
                JsonResult result = ValidateCreate(model, Prescription);
                if (result != null)
                    return result;
                DescriptionModel ToaThuoc = new DescriptionModel()
                {
                    CreateDate = DateTime.Now,
                    CreateBy = CurrentUser.AccountId,
                    DescriptionId = Guid.NewGuid(),
                    Note = model.Note,
                    IsPay = false,
                    DescriptionCode = DataCodeGenerate.KeToaCodeGen(),
                    NumberOfDate = model.DayOfMedicine
                };
                if (model.PatientType == false)
                    ToaThuoc.CreateFor = model.FastSearch;
                else
                {
                    ToaThuoc.AnonymousClient = string.Format("{0} {1}", model.PatientLastName, model.PatientFirstName);
                    ToaThuoc.AnonymousPhone = model.PatientPhone;
                }
                _context.Entry(ToaThuoc).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                foreach (var i in Prescription)
                {
                    var price = _context.WarehouseDetailModels.Where(x => x.WarehouseDetailId == i.WarehouseDetailId).FirstOrDefault();
                    string TempPrice = ((double)price.SalePrice / Convert.ToDouble(price.BoughtQuantity)).ToString("N0");
                    double? Price = ((int)i.PrescriptionNumber * Convert.ToDouble(TempPrice));
                    DescriptionDetailModel ToaDetail = new DescriptionDetailModel()
                    {
                        DescriptionDetailId = Guid.NewGuid(),
                        DescriptionId = ToaThuoc.DescriptionId,
                        HowToUseNote = i.HowToUse,
                        MedicineId = i.WarehouseDetailId,
                        Quantity = i.PrescriptionNumber,
                        TotalPay = Price
                    };
                    _context.Entry(ToaDetail).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                }
                foreach (var j in model.IllnessLst)
                {
                    DescriptionIllnessModel Benhs = new DescriptionIllnessModel()
                    {
                        DescriptionId = ToaThuoc.DescriptionId,
                        DescriptionIllnessId = Guid.NewGuid(),
                        IllnessId = j
                    };
                    _context.Entry(Benhs).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                }

                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Tạo toa thuốc thành công",
                    redirect = "/Services/Prescription"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = ex.Message.ToString()
                });
            }
        }


        public ActionResult Edit(Guid Id)
        {
            var result = _context.DescriptionModels.Where(x => x.DescriptionId == Id).FirstOrDefault();
            if (!string.IsNullOrEmpty(result.AnonymousClient) || !string.IsNullOrEmpty(result.AnonymousPhone))
                ViewBag.PatientType = true;
            else
                ViewBag.PatientType = false;

            //Danh sách thuốc đã kê
            List<DSThuocDetailViewModel> DSThuoc = new List<DSThuocDetailViewModel>();
            foreach (var i in result.DescriptionDetailModels)
            {
                string TempPrice = ((double)i.WarehouseDetailModel.SalePrice / Convert.ToDouble(i.WarehouseDetailModel.BoughtQuantity)).ToString("N0");
                double? Price = ((int)i.Quantity * Convert.ToDouble(TempPrice));
                DSThuoc.Add(new DSThuocDetailViewModel()
                {
                    TenThuoc = i.WarehouseDetailModel.MedicineProvideModel.MedicineModel.MedicineName,
                    NCC = i.WarehouseDetailModel.MedicineProvideModel.ProviderModel.ProviderName,
                    GiaTien = Convert.ToDouble(TempPrice),
                    Note = i.HowToUseNote,
                    SoLuong = i.Quantity,
                    Total = i.TotalPay,
                    WarehouseDetailId = i.WarehouseDetailModel.WarehouseDetailId,
                    DVT = i.WarehouseDetailModel.MedicineProvideModel.MedicineModel.Unit
                });
            }
            ViewBag.DSThuoc = DSThuoc;

            return View(result);
        }

        public ActionResult Detail(Guid Id)
        {
            var bill = _context.DescriptionModels.Where(x => x.DescriptionId == Id).FirstOrDefault();
            ViewBag.ToTal = ((double)bill.DescriptionDetailModels.Sum(x => x.TotalPay)).ToString("N0");
            return View(bill);
        }

        public JsonResult ValidateCreate(PrescriptionCreateViewModel model, List<PrescriptionDetailCreateViewModel> Prescription)
        {
            //Bệnh nhân vãng lai
            if (model.PatientType == true)
            {
                if (string.IsNullOrEmpty(model.PatientFirstName))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập tên bệnh nhân"
                    });
                }
                if (string.IsNullOrEmpty(model.PatientLastName))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập họ bệnh nhân"
                    });
                }
                if (string.IsNullOrEmpty(model.PatientPhone))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập số điện thoại của bệnh nhân"
                    });
                }
            }
            else
            {
                if (model.FastSearch == null || model.FastSearch == Guid.Empty)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng chọn bệnh nhân"
                    });
                }
            }
            if (model.DayOfMedicine <= 0 || model.DayOfMedicine == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn số ngày thuốc cho bệnh nhân"
                });
            }
            if (model.IllnessLst.Count <= 0 || model.IllnessLst == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn ít nhất 1 bệnh mà bệnh nhân bị"
                });
            }
            if (Prescription.Count <= 0 || Prescription == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn ít nhất 1 thuốc cho bệnh nhân"
                });
            }
            foreach (var i in Prescription)
            {
                if (i.PrescriptionNumber == null || i.PrescriptionNumber <= 0)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không để trống số lượng thuốc trong danh mục thuốc cần kê"
                    });
                }
                if (i.WarehouseDetailId == null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không hack web"
                    });
                }
            }
            return null;
        }
    
        public JsonResult ClientSearch(string searchTerm)
        {
            List<SelectListGuidForAutoComplete> ClientList = new List<SelectListGuidForAutoComplete>();
            //Danh sách khách hàng có tài khoản
            var UserWithAccount = _context.AccountModels.Where(x => (x.UsersModel.FirstName.Contains(searchTerm) || x.UsersModel.LastName.Contains(searchTerm) || x.UserName.Contains(searchTerm))).
                Select(x => new SelectListGuidForAutoComplete
                {
                    text = x.UsersModel.LastName + " " + x.UsersModel.FirstName,
                    value = x.AccountId
                    
                }).Take(10).ToList();
            ClientList.AddRange(UserWithAccount);

            return Json(ClientList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteProvider(string kq)
        {
            List<string> ProLst = new List<string>();
            ProLst.AddRange(_context.ProviderModels.Where(x => x.ProviderName.Contains(kq)).Select(x => x.ProviderName).Take(10).ToList());
            return Json(ProLst, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddNewMedi(Guid WarehouseDetailId, int? InstockQuantity, int Index)
        {
            var result = _context.WarehouseDetailModels.Where(x => x.WarehouseDetailId == WarehouseDetailId).FirstOrDefault();
            if (InstockQuantity <= 0 || InstockQuantity == null)
            {
                return Json("Vui lòng chọn số lượng cần kê");
            }
            if (result.InstockQuantity < InstockQuantity)
                return Json("Vui lòng không nhập số lượng lớn hơn số lượng còn tồn");
            ViewBag.SoLuongKe = InstockQuantity;
            string TempPrice = ((double)result.SalePrice / Convert.ToDouble(result.BoughtQuantity)).ToString("N0");
            ViewBag.Price = TempPrice;
            ViewBag.Total = ((int)InstockQuantity * Convert.ToDouble(TempPrice)).ToString("N0");
            ViewBag.Index = Index;
            return PartialView(result);
        }


        [HttpPost]
        public JsonResult GetInfo(Guid ClientId)
        {
            var info = _context.AccountModels.Where(x => x.AccountId == ClientId).FirstOrDefault();
            return Json(new 
            {
                LastName = info.UsersModel.LastName,  
                FirstName = info.UsersModel.FirstName,
                Phone = info.UsersModel.Phone,
                Birthday = info.UsersModel.Birthday != null ? ((DateTime)info.UsersModel.Birthday).ToString("yyyy-MM-dd") : null,
                Address = info.UsersModel.Address,
                Email = info.UsersModel.Email
            });
        }


        public JsonResult IllnessSearch(string searchTerm)
        {
            List<SelectListGuidForAutoComplete> IllnessList = new List<SelectListGuidForAutoComplete>();
            //Danh sách khách hàng có tài khoản
            var Illness = _context.IllnessModels.Where(x => x.IllnessName.Contains(searchTerm)).Select(x =>
            new SelectListGuidForAutoComplete
            {
                text = x.IllnessName,
                value = x.IllnessId
            }).Take(10).ToList();
            IllnessList.AddRange(Illness);

            return Json(IllnessList, JsonRequestBehavior.AllowGet);
        }


    }
}