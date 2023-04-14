using System;

namespace EnjuAihara.ViewModels.Services
{
    public class PrescriptionDetailCreateViewModel
    {
        public Guid WarehouseDetailId { get; set; }

        public int? PrescriptionNumber { get; set; }

        public string HowToUse { get; set; }
    }
}
