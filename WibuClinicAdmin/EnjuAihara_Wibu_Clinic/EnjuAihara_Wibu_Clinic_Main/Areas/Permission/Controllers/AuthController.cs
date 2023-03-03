using EnjuAihara.Core;
using EnjuAihara.ViewModels.Permission.Auth;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnjuAihara.Utilities.EncryptionAlgorithm;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using EnjuAihara.EntityFramework;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Permission.Controllers
{
    public class AuthController : IrinaLumineskController
    {
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

    }
}