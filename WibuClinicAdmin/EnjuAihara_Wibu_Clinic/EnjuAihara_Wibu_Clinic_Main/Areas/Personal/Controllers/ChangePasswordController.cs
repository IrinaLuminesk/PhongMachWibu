using EnjuAihara.Core;
using EnjuAihara.Utilities.EncryptionAlgorithm;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Personal.Controllers
{
    public class ChangePasswordController : IrinaLumineskController
    {
        // GET: Personal/ChangePassword
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Edit(string OldPassword, string NewPassword)
        {
            try
            {
                Guid CurrentId = CurrentUser.AccountId;
                var account = _context.AccountModels.Where(x => x.AccountId == CurrentId).FirstOrDefault();
                var OldPassEncrypt = Encrypt.SHA256Encrypt(OldPassword);
                if (string.IsNullOrEmpty(OldPassword))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập mật khẩu cũ",
                    });
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập mật khẩu mới",
                    });
                }
                if (!account.Password.Equals(OldPassEncrypt))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Mật khẩu mới không trùng với mật khẩu cũ",
                    });
                }
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                if (hasMinimum8Chars.IsMatch(NewPassword) == false || hasUpperChar.IsMatch(NewPassword) == false || hasNumber.IsMatch(NewPassword))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng đặt mật khẩu đúng định dạng",
                    });
                }
                account.Password = Encrypt.SHA256Encrypt(NewPassword);
                _context.Entry(account).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Đổi mật khẩu thành công vui lòng đăng nhập lại",
                    redirect = "/Permission/Auth/Login"
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