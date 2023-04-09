using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.CloudinaryHelper;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.Utilities.RandomString;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public JsonResult _PaggingServerSide(DatatableViewModel model, UserViewModel search, string UserCode, string UserName, DateTime? BirthdayFrom, DateTime? BirthdayTo, string Address, string Email, string Phone, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (BirthdayTo != null)
                BirthdayTo = ((DateTime)BirthdayTo).AddDays(1).AddSeconds(-1);

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.UsersModels.Where(x =>
            (x.UserCode.Contains(UserCode) || string.IsNullOrEmpty(UserCode)) &&
            ((x.LastName.Contains(UserName) || string.IsNullOrEmpty(UserName)) ||
            (x.FirstName.Contains(UserName) || string.IsNullOrEmpty(UserName))) &&
            (x.Birthday >= BirthdayFrom || BirthdayFrom == null) &&
            (x.Birthday <= BirthdayTo || BirthdayTo == null) &&
            (x.Address.Contains(Address) || string.IsNullOrEmpty(Address)) &&
            (x.Phone.Contains(Phone) || string.IsNullOrEmpty(Phone)) &&
            (x.Email.Contains(Email) || string.IsNullOrEmpty(Email)) &&
            (x.Actived == Actived || Actived == null)
            )
            .Select(x =>
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
            })
            
            .OrderBy(x => x.UserCodeResult).ToList();

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


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Create(EditUserViewModel model)
        {
            try
            {
                JsonResult result = ValidateUser(model);
                if (result != null)
                    return result;
                var newuser = new UsersModel()
                {
                    UserID = Guid.NewGuid(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ImagePath = model.Avatar == null ? "" : CloudinaryUpload.Upload(model.Avatar),
                    Actived = true,
                    Address = model.Address,
                    Birthday = model.Birthday,
                    Email = model.Email,
                    Phone = model.Phone,
                    UserCode = DataCodeGenerate.NguoiDungCodeGen()
                };
                _context.Entry(newuser).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thêm thành công",
                    message = "Thêm người dùng mới thành công",
                    redirect = "/MasterData/User"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra trong quá trình tạo người dùng " + ex.Message.ToString()
                });
            }
        }


        public ActionResult Edit(Guid id)
        {
            var user = _context.UsersModels.Where(x => x.UserID == id).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        public JsonResult Edit(EditUserViewModel model)
        {
            JsonResult result = ValidateUser(model);
            if (result != null)
                return result;
            var editUser = _context.UsersModels.Where(x => x.UserID == model.UserID).FirstOrDefault();
            editUser.LastName = model.LastName;
            editUser.FirstName = model.FirstName;
            if(model.Avatar != null)
                editUser.ImagePath = CloudinaryUpload.Upload(model.Avatar);
            editUser.Phone = model.Phone;
            editUser.Email = model.Email;
            editUser.Address = model.Address;
            model.Actived = model.Actived;
            editUser.Birthday = model.Birthday;
            _context.SaveChanges();
            return Json(new
            {
                isSucess = true,
                title = "Sửa thành công",
                message = string.Format("Sửa {0} thành công", editUser.LastName + " " + editUser.FirstName),
                redirect = "/MasterData/User"
            });
        }

        public JsonResult ValidateUser(EditUserViewModel model)
        {
            if (model.ImagePath != null)
            {
                if (CloudinaryUpload.CheckFileExtension(Path.GetExtension(model.Avatar.FileName)) == false)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng chọn ảnh đúng định dạng"
                    });
                }
            }
            if (string.IsNullOrEmpty(model.FirstName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên người dùng"
                });
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống họ người dùng"
                });
            }
            if (string.IsNullOrEmpty(model.Address))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống địa chỉ người dùng"
                });
            }

            if (string.IsNullOrEmpty(model.Phone))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống điện thoại người dùng"
                });
            }
            else
            {
                bool flag = int.TryParse(model.Phone, out _);
                if (flag == false)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập đúng định dạng số điện thoại"
                    });
                }
                if (model.Phone.Length > 10)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập đúng định dạng số điện thoại"
                    });
                }
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống Email người dùng"
                });
            }
            else
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập đúng định dạng Email người dùng"
                    });
                }
            }
            if (model.Birthday == null)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống ngày sinh người dùng"
                });
            }
            else
            {
                if (model.Birthday > DateTime.Now)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không nhập ngày sinh người dùng lớn hơn ngày hiện tại"
                    });
                }
            }
            return null;
        }
    }
}