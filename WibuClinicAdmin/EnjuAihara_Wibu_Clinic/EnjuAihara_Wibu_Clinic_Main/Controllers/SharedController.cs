using EnjuAihara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EnjuAihara.ViewModels.Permission;
using EnjuAihara.ViewModels.Firebase;

namespace EnjuAihara_Wibu_Clinic_Main.Controllers
{
    public class SharedController : IrinaLumineskController
    {
        // GET: Shared
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Sidebar()
        {
            CreateConfigViewBag();


            var AllCurrentUserRoles = CurrentUser.AccountInRoleModels.Select(x => x.RolesModel).ToList();

            List<PageViewModel> PermissionPages = new List<PageViewModel>();
            foreach (var i in AllCurrentUserRoles)
            {
                PermissionPages.AddRange(_context.PagePermissionModels.Where(x => x.RoleId == i.RoleId && x.FuntionId.Equals("INDEX")).Select(x =>
                new PageViewModel
                {
                    PageName = x.PageModel.PageName,
                    OrderIndex = (int)x.PageModel.OrderIndex,
                    Icon = x.PageModel.Icon,
                    PageUrl = x.PageModel.PageUrl
                }).ToList());
            }
            ViewBag.CurrentUser = CurrentUser;
            var result = _context.MenuModels.Where(x => x.Actived == true).OrderByDescending(x => x.OrderIndex).ToList();
            List<MenuViewModel> finalresult = new List<MenuViewModel>();
            int y = 0;
            foreach (var j in result)
            {
                finalresult.Add(new MenuViewModel()
                {
                    Icon = j.Icon,
                    MenuName = j.MenuName,
                    OrderIndex = j.OrderIndex,
                    Pages = new List<PageViewModel>()
                });
                
                foreach(var u in j.PageModels)
                {
                    if (PermissionPages.Any(x => x.PageName.Equals(u.PageName)))
                    {
                        finalresult[y].Pages.Add(new PageViewModel()
                        {
                            OrderIndex = u.OrderIndex, 
                            Icon = u.Icon,
                            PageName = u.PageName,
                            PageUrl = u.PageUrl,
                            Actived = u.Actived
                        });
                    }
                }
                y++;
            }

            return PartialView(finalresult);
        }

        public List<NotificationViewModel> GetAllUserNotification()
        {
            var CurrentId = CurrentUser.AccountId;
            List<NotificationViewModel> list = new List<NotificationViewModel>();
            list.AddRange(_context.NotificationForAccounts.Where(x => x.AccountId == CurrentId && x.IsRead == false).OrderBy(x => x.NotificationModel.CreateDate).Select(x =>
            new NotificationViewModel
            {
               Content = x.NotificationModel.NotificationDetail,
               CreateDate = x.NotificationModel.CreateDate,
               NotificationId = x.NotificationModel.NotificationId,
               Title = x.NotificationModel.NotificationTitle,
               Link = x.NotificationModel.NotificationLink
            }).ToList());
            return list;
        }

        public ActionResult Error()
        {
            return View();
        }


        public PartialViewResult Notification()
        {
            var result = GetAllUserNotification();
            return PartialView(result);
        }

        public PartialViewResult NotificationCount()
        {
            var result = GetAllUserNotification();
            if (result != null && result.Count > 0)
            {
                return PartialView(result.Count);
            }
            else
            {
                return PartialView(0);
            }
        }


        [HttpPost]
        public PartialViewResult GetNewNotification(Guid Id)
        {
            var Notification = _context.NotificationModels.Where(x => x.NotificationId == Id).FirstOrDefault();
            return PartialView(Notification);
        }


        public void CreateConfigViewBag()
        {
            //Mở Inspect Element
            ViewBag.InspectElement = Convert.ToBoolean(_context.CatalogModels.Where(x => x.CatalogCode.Equals("InspectElement")).FirstOrDefault().Value);
        }
    }
}