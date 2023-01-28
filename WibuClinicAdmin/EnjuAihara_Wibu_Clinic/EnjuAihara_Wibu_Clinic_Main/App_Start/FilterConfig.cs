using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
