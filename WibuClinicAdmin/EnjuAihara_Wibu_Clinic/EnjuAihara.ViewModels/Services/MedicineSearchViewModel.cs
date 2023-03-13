using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Services
{
    public class MedicineSearchViewModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public int STT { get; set; }

        public string ProName { get; set; }

        public string MedName { get; set; }
        public double? Price { get; set; }

        public string Status { get; set; }

        public Guid? WarehouseId { get; set; }
        public Guid? MedicineProviderId { get; set; }

        public string Unit { get; set; }

        public decimal? SoLuongTon { get; set; }

        public DateTime? HanSuDung { get; set; }

        public string HanSuDungString { get; set; }
    }
}
