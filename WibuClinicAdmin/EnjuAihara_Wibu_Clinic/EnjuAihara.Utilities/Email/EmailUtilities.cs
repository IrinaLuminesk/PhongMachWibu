using EnjuAihara.EntityFramework;
using System;
using System.Web.Configuration;

namespace EnjuAihara.Utilities.Email
{
    public static class EmailUtilities
    {
        public static EmailSenderModel PasswordResetMail(AccountModel Account)
        {
            string Url = WebConfigurationManager.AppSettings["WebUrl"];
            string Control = string.Format("Permission/PasswordReset/{0}", Account.AccountId.ToString());
            EmailSenderModel model = new EmailSenderModel()
            {
                EmailId = Guid.NewGuid(),
                SendTo = Account.AccountId,
                Content = string.Format("<h1> Vui lòng nhấn vào đường dẫn để reset mật khẩu tài khoản <a>{0}</a> <h1>", Url + Control),
                CreateDate = DateTime.Now,
                EmailType = "PASSWORDRESET",
                IsSend = false,
                Title = string.Format("Khôi phục mật khẩu cho tài khoản {0}", Account.UserName)
            };
            return model;
        }


    }
}
