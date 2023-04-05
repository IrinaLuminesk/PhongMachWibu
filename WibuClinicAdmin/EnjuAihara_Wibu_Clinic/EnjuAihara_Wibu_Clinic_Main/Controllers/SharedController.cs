using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            //var CurrentId = CurrentUser.AccountId;
            //var AllRole = CurrentUser.AccountInRoleModels.ToList();
            //var list = _context.NotificationModels.Where(x => x.NotificationFor == CurrentId && x.IsRead == false).OrderBy(x => x.CreateDate).Select
            //    (x => new NotificationViewModel
            //    {
            //        NotificationId = x.NotificationId,
            //        Content = x.NotificationDetail,
            //        Title = x.NotificationTitle,
            //        CreateDate = x.CreateDate
            //    }).ToList();
            //var list2 = new List<NotificationViewModel>();
            //foreach (var i in AllRole)
            //{
            //    list2.AddRange(_context.NotificationModels.Where(x => x.NotificationInRoleModels.Any(y => y.RoleId == i.RoleId)).Select
            //    (x => new NotificationViewModel
            //    {
            //        NotificationId = x.NotificationId,
            //        Content = x.NotificationDetail,
            //        Title = x.NotificationTitle,
            //        CreateDate = x.CreateDate
            //    }).ToList());
            //}
            //List<NotificationViewModel> FinalList = new List<NotificationViewModel>();
            //FinalList.AddRange(list2);
            //FinalList.AddRange(list);
            //return FinalList;
            return null;
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
    }
}