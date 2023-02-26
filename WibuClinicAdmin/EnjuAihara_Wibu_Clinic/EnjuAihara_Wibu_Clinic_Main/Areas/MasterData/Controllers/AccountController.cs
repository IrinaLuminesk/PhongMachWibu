using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.Utilities.CloudinaryHelper;
using EnjuAihara.Utilities.EncryptionAlgorithm;
using System.IO;
using Microsoft.AspNet.Identity;
using EnjuAihara.Utilities.RandomString;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class AccountController : IrinaLumineskController
    {

        // GET: MasterData/Account
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public JsonResult _PaggingServerSide(DatatableViewModel model, AccountSearchViewModel search, string AccountCodeSearch, string AccountName, Guid? RoleNameSearch, DateTime? FromDate, DateTime? ToDate, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (ToDate != null)
                ToDate = ((DateTime)ToDate).AddDays(1).AddSeconds(-1);

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.AccountModels.
                Where(x => (x.AccountCode.Contains(AccountCodeSearch) || string.IsNullOrEmpty(AccountCodeSearch))
                && (x.UserName.Contains(AccountName) || string.IsNullOrEmpty(AccountName))
                && (x.AccountInRoleModels.Any(z => z.RoleId == RoleNameSearch) || RoleNameSearch == null)
                && (x.Actived == Actived || Actived == null)
                && (x.CreateDate >= FromDate || FromDate == null)
                && (x.CreateDate <= ToDate || ToDate == null)
                )
                .Select(x =>
            new AccountSearchViewModel
            {
                AccountId = x.AccountId,
                AccountCode = x.AccountCode,
                UserName = x.UserName,
                //RoleName = x.RolesModels.Select(y => y.RoleName).ToList(),
                RoleName = x.AccountInRoleModels.Select(y => y.RolesModel.RoleName).ToList(),
                CreateDate = x.CreateDate,
                RealName = x.UsersModel.LastName + " " + x.UsersModel.FirstName,
                CreateBy = x.AccountModel2.UserName,
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngưng",
                LastLoginTime = x.LastLoginTime

            }).OrderBy(x => x.CreateDate).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<AccountSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.CreateDateString = FormatDateTime.FormatDateTimeWithString(item.CreateDate);
                    item.LastLoginTimeString = FormatDateTime.FormatDateTimeWithString(item.LastLoginTime);
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
            var RoleList = _context.RolesModels.Select(x =>
            new SelectGuidItem
            {
                id = x.RoleId,
                name = x.RoleName
            }).ToList();
            ViewBag.RoleNameSearch = new SelectList(RoleList, "id", "name");

            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }

        public void CreateRolesViewBag(Guid? Id)
        {
            var RoleList = _context.RolesModels.ToList();
            ViewBag.RoleList = RoleList;

            if (Id != null)
            {
                //var SelectRoleList = _context.AccountModels.Where(x => x.AccountId == Id).Select(x => x.RolesModels.ToList()).FirstOrDefault();
                var SelectRoleList = _context.AccountInRoleModels.Where(x => x.AccountId == Id).Select(x => x.RolesModel.RoleId).ToList();
                ViewBag.SelectRoleList = SelectRoleList;
            }
        }

        public ActionResult Create()
        {
            CreateRolesViewBag(null);
            return View();
        }

        [HttpPost]
        public JsonResult Create(AccountCreateViewModel model)
        {
            JsonResult ValidateResult = ValidateAccount(model);
            if (ValidateResult != null)
                return ValidateResult;
            try
            {

                //Lưu tài khoản
                AccountModel newAccount = new AccountModel()
                {
                    AccountId = Guid.NewGuid(),
                    AccountCode = DataCodeGenerate.UserCodeGen(),
                    Actived = true,
                    CreateBy = CurrentUser.AccountId,
                    CreateDate = DateTime.Now,
                    ImagePath = model.Avatar == null ? null : CloudinaryUpload.Upload(model.Avatar),
                    LastLoginTime = DateTime.Now,
                    Password = Encrypt.SHA256Encrypt(model.Password),
                    UserName = model.UserName,
                    UserId = model.ForUser
                };
                _context.Entry(newAccount).State = EntityState.Added;
                _context.SaveChanges();


                //Lưu nhóm người dùng

                List<AccountInRoleModel> roles = new List<AccountInRoleModel>();
                foreach (var i in model.Roles)
                {
                    roles.Add(new AccountInRoleModel() { AccountRoleId = Guid.NewGuid(), AccountId = newAccount.AccountId, RoleId = i });
                }
                _context.AccountInRoleModels.AddRange(roles);
                _context.SaveChanges();

                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Tạo tài khoản mới thành công"
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


        public ActionResult Edit(Guid? Id)
        {

            var model = _context.AccountModels.Where(x => x.AccountId == Id).FirstOrDefault();
            CreateRolesViewBag(Id);
            return View(model);
        }


        [HttpPost]
        public JsonResult Edit(AccountEditViewModel model)
        {
            bool flag = true;
            JsonResult ValidateResult = ValidateAccountEdit(model);
            if (ValidateResult != null)
                return ValidateResult;
            try
            {
                var EditAccount = _context.AccountModels.Where(x => x.AccountId == model.AccountId).FirstOrDefault();
                if (EditAccount == null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Đã có lỗi xảy ra"
                    });
                }
                if (EditAccount.AccountId == CurrentUser.AccountId)
                    flag = false;
                EditAccount.UserName = model.UserName;
                EditAccount.Password = Encrypt.SHA256Encrypt(model.Password);
                if (model.Avatar != null)
                {
                    EditAccount.ImagePath = CloudinaryUpload.Upload(model.Avatar);
                }

                EditAccount.Actived = model.Actived;


                var listRoles = _context.AccountInRoleModels.Where(x => x.AccountId == model.AccountId).ToList();
                _context.AccountInRoleModels.RemoveRange(listRoles);
                _context.SaveChanges();
                List<AccountInRoleModel> roles = new List<AccountInRoleModel>();
                foreach (var i in model.Roles)
                {
                    roles.Add(new AccountInRoleModel() { AccountRoleId = Guid.NewGuid(), AccountId = EditAccount.AccountId, RoleId = i });
                }
                _context.AccountInRoleModels.AddRange(roles);
                _context.SaveChanges();
                if (flag == false)
                {
                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return Json(new
                    {
                        isSucess = true,
                        title = "Sửa thành công",
                        message = string.Format("Sửa {0} thành công, vui lòng đăng nhập lại", EditAccount.UserName),
                        redirect = "/Permission/Auth/Login"
                    });
                }
                return Json(new
                {
                    isSucess = true,
                    title = "Sửa thành công",
                    message = string.Format("Sửa {0} thành công", EditAccount.UserName),
                    redirect = "/MasterData/Account"
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

        public JsonResult ValidateAccount(AccountCreateViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên tài khoản"
                });
            }
            else
            {
                var result = _context.AccountModels.Where(x => x.UserName.Equals(model.UserName)).FirstOrDefault();
                if (result != null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Tên tài khoản đã tồn tại, vui lòng chọn tên khác"
                    });
                }
            }
            if (!model.ForUser.HasValue)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn người dùng bạn muốn tạo tài khoản"
                });
            }

            if (model.Roles == null || model.Roles.Count == 0)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn ít nhất 1 nhóm người dùng cho tài khoản"
                });
            }
            if (string.IsNullOrEmpty(model.Password) || !model.Password.Equals("123456789@abcd"))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không thay đổi mật khẩu mặc định"
                });
            }

            if (model.Avatar != null)
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
            return null;
        }

        public JsonResult ValidateAccountEdit(AccountEditViewModel model)
        {
            if (model.Avatar != null)
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
            if (string.IsNullOrEmpty(model.UserName))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng không để trống tên tài khoản"
                });
            }
            else
            {
                var CurrentEditAccount = _context.AccountModels.Where(x => x.AccountId == model.AccountId).FirstOrDefault();
                var result = _context.AccountModels.Where(x => x.UserName.Equals(model.UserName)).FirstOrDefault();
                if (result != null)
                {
                    if (!result.UserName.Equals(CurrentEditAccount.UserName))
                    {
                        return Json(new
                        {
                            isSucess = false,
                            title = "Lỗi",
                            message = "Tên tài khoản đã tồn tại, vui lòng chọn tên khác"
                        });
                    }
                }
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                model.Password = "123456789@abcd";
                if (!model.Password.Equals("123456789@abcd"))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không thay đổi mật khẩu mặc định"
                    });
                }
            }
            if (model.Roles == null || model.Roles.Count == 0)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Vui lòng chọn ít nhất 1 nhóm người dùng cho tài khoản"
                });
            }
            return null;
        }

        public JsonResult AutoComplete(string searchTerm)
        {
            var result = _context.UsersModels.Where(x => 
            (x.UserCode.Contains(searchTerm) || x.FirstName.Contains(searchTerm) || x.LastName.Contains(searchTerm)) && x.Actived == true
            ).Select(x =>
            new
            {
                value = x.UserID,
                text = x.UserCode + " | " + x.LastName + " " + x.FirstName
            }).Take(20).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}