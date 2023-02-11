﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class AccountEditViewModel
    {
        public HttpPostedFileBase Avatar { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public List<Guid> Roles { get; set; }

        public bool Actived { get; set; }

        public Guid AccountId { get; set; }
    }
}
