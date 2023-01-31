using EnjuAihara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    public class AuthController : IrinaLumineskController
    {
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }
    }
}