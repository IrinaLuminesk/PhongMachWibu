using EnjuAihara.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class EditUserViewModel : UsersModel
    {
        public HttpPostedFileBase Avatar { get; set; }
    }
}
