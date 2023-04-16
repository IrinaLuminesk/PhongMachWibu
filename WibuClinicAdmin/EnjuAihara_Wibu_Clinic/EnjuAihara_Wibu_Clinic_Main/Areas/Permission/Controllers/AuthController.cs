using EnjuAihara.Core;
using EnjuAihara.ViewModels.Permission.Auth;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.Utilities.EncryptionAlgorithm;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Email;
using System.Data.Entity.Validation;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class AuthController : IrinaLumineskController
    {
        public EmailSenderModel EmailUtility { get; private set; }

        //Login Page
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            var i = CurrentUser;
                HttpCookie userInfo = Request.Cookies["userInfo"];
                LoginViewModel log = new LoginViewModel()
                {
                    RememberMe = false,
                    Password = "",
                    Username = ""
                };
                if (userInfo != null)
                {
                    log.RememberMe = true;
                    log.Username = userInfo["Username"].ToString();
                    log.Password = userInfo["Password"].ToString();
                }
                return View("Login", log);

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập và mật khẩu để sử dụng hệ thống");
                return View();
            }
            if (string.IsNullOrEmpty(model.Username))
            {
                ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập");
                return View();
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Vui lòng nhập mật khẩu");
                return View();
            }
            if (!string.IsNullOrEmpty(model.Username))
            {
                if (CheckLogin(model) == true)
                {
                    if(!string.IsNullOrEmpty(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return Redirect("/");
                }
            }
            return View();
        }


        public bool CheckLogin(LoginViewModel model)
        {
            string username = model.Username.Trim();
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Vui lòng nhập mật khẩu");
                return false;
            }
            string password = Encrypt.SHA256Encrypt(model.Password);
            var account = _context.AccountModels.Where(x => x.UserName.Equals(username) && x.Password.Equals(password)).FirstOrDefault();
            if (account == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại");
                return false;
            }

            if (account.AccountInRoleModels.Count() == 1)
            {
                if (account.AccountInRoleModels.Any(x => x.RolesModel.RoleName.Equals("Khách hàng")))
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                    return false;
                }
            }

            if (model.RememberMe == true)
            {
                HttpCookie userInfo = new HttpCookie("userInfo");
                userInfo.HttpOnly = true;
                userInfo["Username"] = model.Username;
                userInfo["Password"] = model.Password;
                userInfo.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(userInfo);
                Request.Cookies["userInfo"].Expires = DateTime.Now.AddDays(30);
            }
            var identity = new ClaimsIdentity(new[] 
            {
                //Username
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Sid, account.AccountId.ToString())
            },
            DefaultAuthenticationTypes.ApplicationCookie);
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignIn(identity);
            account.LastLoginTime = DateTime.Now;
            _context.SaveChanges();

            //Lấy các quyền của tài khoản 
            //var AllRole = account.AccountInRoleModels.Select(x => x.RoleId).ToList();
            //List<PagePermissionModel> Permissions = new List<PagePermissionModel>();
            //foreach (var i in AllRole)
            //{
            //    Permissions.AddRange(_context.PagePermissionModels.Where(x => x.RoleId == i).ToList());
            //}
            //Session["Permission"] = Permissions;
            return true;
        }


        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect("/Permission/Auth/Login");
        }


        [AllowAnonymous]
        public ActionResult PasswordReset()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult PasswordReset(string Username)
        {
            try
            {
                var account = _context.AccountModels.Where(x => x.UserName.Equals(Username)).FirstOrDefault();
                if (account == null)
                    return Json("Tài khoản không tồn tại");
                Guid Id = Guid.NewGuid();
                AccountRecoveryTokenModel token = new AccountRecoveryTokenModel()
                {
                    AccountId = account.AccountId,
                    ExpiredDate = DateTime.Now.AddMinutes(10),
                    TokenId = Id,
                    Token = string.Format("{0}{1}", account.AccountId.ToString(), Id.ToString())
                };
                _context.Entry(token).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();

                EmailSenderModel email = EmailUtilities.PasswordResetMail(account, token.Token);
                _context.Entry(email).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json("Vui lòng làm theo hướng dẫn được gửi tới email của tài khoản để khôi phục mật khẩu");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return Json(ex.Message.ToString());
            }
        }


        [AllowAnonymous]
        public ActionResult PasswordChange(string Id)
        {
            var token = _context.AccountRecoveryTokenModels.Where(x => x.Token.Equals(Id)).FirstOrDefault();
            if (token == null)
                return Redirect("/Permission/Auth/Error");
            if (token.ExpiredDate < DateTime.Now)
                return Redirect("/Permission/Auth/Error");
            ViewBag.Token = token.Token;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PasswordChange(string NewPassword, string Token)
        {
            if(string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(NewPassword))
                return Redirect("/Permission/Auth/Error");
            var token = _context.AccountRecoveryTokenModels.Where(x => x.Token.Equals(Token)).FirstOrDefault();
            var account = token.AccountModel;
            account.Password = Encrypt.SHA256Encrypt(NewPassword);
            _context.Entry(account).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            return Redirect("/Permission/Auth/Login");
        }

    }
}