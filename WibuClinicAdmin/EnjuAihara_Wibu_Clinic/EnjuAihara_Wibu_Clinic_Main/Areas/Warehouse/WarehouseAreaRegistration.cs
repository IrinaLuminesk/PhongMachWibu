using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.Warehouse
{
    public class WarehouseAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Warehouse";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Warehouse_default",
                "Warehouse/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}