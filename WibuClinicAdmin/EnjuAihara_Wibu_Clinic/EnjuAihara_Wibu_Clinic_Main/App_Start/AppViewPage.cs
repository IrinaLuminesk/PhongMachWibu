using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.App_Start
{
    public abstract class AppViewPage<TModel> : WebViewPage<TModel>
    {
        
    }
    public abstract class AppViewPage : AppViewPage<dynamic>
    {
    }
}