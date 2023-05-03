using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using EnjuAihara.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace EnjuAihara.Firebase
{
    public class FirebaseUtility
    {
        public static IFirebaseClient GetFireBaseClient()
        {
            IFirebaseConfig config = new FirebaseConfig()
            {
                AuthSecret = "AIzaSyBkyc7R6BV4cVg3DpL2RmgWKsuvuH5GoIM",
                BasePath = "https://quanlyphongmachwibu-default-rtdb.firebaseio.com"
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }


        public static void InsertData(string Title, string Message, string Link)
        {
            try
            {
                QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
                IFirebaseClient client = GetFireBaseClient();
                var Notification = new NotificationModel()
                {
                    NotificationId = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    NotificationDetail = Message,
                    NotificationLink = Link,
                    NotificationTitle = Title,
                };
                _context.Entry(Notification).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                foreach (var i in _context.AccountModels.Where(x => x.AccountInRoleModels.Any(y => y.RolesModel.RoleCode.Equals("SYSADMIN") || y.RolesModel.RoleCode.Equals("ADMIN"))).ToList())
                {
                    var NotificationFor = new NotificationForAccount()
                    {
                        AccountId = i.AccountId,
                        IsRead = false,
                        NotificationId = Notification.NotificationId,
                        NotificationForAccountId = Guid.NewGuid()
                    };
                    _context.Entry(NotificationFor).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                }
                FirebaseResponse res = client.Push("/", Notification.NotificationId);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
