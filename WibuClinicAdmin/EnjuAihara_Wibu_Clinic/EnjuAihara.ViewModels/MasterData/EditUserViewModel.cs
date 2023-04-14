using EnjuAihara.EntityFramework;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class EditUserViewModel : UsersModel
    {
        public HttpPostedFileBase Avatar { get; set; }
    }
}
