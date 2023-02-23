using EnjuAihara.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class MedicineCreateViewModel
    {
        public string MedicineName { get; set; }
        public string MedicineCode { get; set; }
        public string Unit { get; set; }
        public List<Guid> Ingredient { get; set; }
        public List<HttpPostedFileBase> DrugImage { get; set; }
        public List<Guid> Provider { get; set; }
    }
}
