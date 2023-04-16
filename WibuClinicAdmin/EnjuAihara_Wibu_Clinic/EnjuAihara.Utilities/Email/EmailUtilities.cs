using EnjuAihara.EntityFramework;
using System;
using System.Web.Configuration;

namespace EnjuAihara.Utilities.Email
{
    public static class EmailUtilities
    {
        public static EmailSenderModel PasswordResetMail(AccountModel Account, string Token)
        {
            string Url = WebConfigurationManager.AppSettings["WebUrl"];
            string Control = string.Format("Permission/Auth/PasswordChange/{0}", Token.ToString());
            EmailSenderModel model = new EmailSenderModel()
            {
                EmailId = Guid.NewGuid(),
                SendTo = Account.AccountId,
                Content = string.Format("<p> Vui lòng nhấn vào <a href='{0}' target='_blank'>đây</a> để reset mật khẩu tài khoản<p>", Url + Control),
                CreateDate = DateTime.Now,
                EmailType = "PASSWORDRESET",
                IsSend = false,
                Title = string.Format("Khôi phục mật khẩu cho tài khoản {0}", Account.UserName),
                SendDate = null
            };
            return model;
        }





    }
}
