using System;
using System.Collections.Generic;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class AccountCreateViewModel
    {
        public Guid? ForUser { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public HttpPostedFileBase Avatar { get; set; }

        public List<Guid> Roles { get; set; }
    }
}
