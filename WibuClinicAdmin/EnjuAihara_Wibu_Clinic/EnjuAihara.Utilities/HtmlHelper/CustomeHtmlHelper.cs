using EnjuAihara.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace System.Web.Mvc.Html
{
    public static class CustomeHtmlHelper
    {
        public static MvcHtmlString ActivedRadioButton<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            result.AppendFormat("<label class=\"col-form-label\">{0} {1}</label><label class=\"col-form-label\" style=\"margin-left:50px\">{2} {3}</label>",
                helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
                "Đang sử dụng",
                helper.RadioButtonFor(expression, false),
               "Ngừng sử dụng");

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString ActivedRadioButtonCustomeText<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string text1, string text2)
        {
            var result = new StringBuilder();
            result.AppendFormat("<label class=\"col-form-label\">{0} {1}</label><label class=\"col-form-label\" style=\"margin-left:50px\">{2} {3}</label>",
                helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
                text1,
                helper.RadioButtonFor(expression, false),
               text2);

            return MvcHtmlString.Create(result.ToString());
        }

        public static bool CheckPermission(string PageUrl, string Function)
        {
            string FormatUrl = string.Format("{0}", PageUrl);
            List<PagePermissionModel> Permissions = (List<PagePermissionModel>)HttpContext.Current.Session["Permission"];
            if (Permissions != null)
            {
                List<PagePermissionModel> temp = Permissions.Where(x => x.PageModel.PageUrl.Equals(FormatUrl)).ToList();
                if (temp.Any(x => x.FuntionId.Equals(Function.ToUpper())))
                    return true;
                return false;
            }
            return true;
        }
    }
}
