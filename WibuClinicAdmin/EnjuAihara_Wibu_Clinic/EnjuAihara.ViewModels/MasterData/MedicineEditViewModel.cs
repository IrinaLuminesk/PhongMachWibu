using System;
using System.Collections.Generic;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class MedicineEditViewModel
    {

        public Guid MedicineProvideId { get; set; }
        public HttpPostedFileBase Img { get; set; }

        public List<Guid> Ingredient { get; set; }

        public Guid Provider { get; set; }

        public bool Actived { get; set; }
    }
}
