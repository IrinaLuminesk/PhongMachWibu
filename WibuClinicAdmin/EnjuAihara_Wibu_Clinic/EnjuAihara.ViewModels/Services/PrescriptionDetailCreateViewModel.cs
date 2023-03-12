using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Services
{
    public class PrescriptionDetailCreateViewModel
    {
        public Guid WarehouseDetailId { get; set; }

        public int? PrescriptionNumber { get; set; }

        public string HowToUse { get; set; }
    }
}
