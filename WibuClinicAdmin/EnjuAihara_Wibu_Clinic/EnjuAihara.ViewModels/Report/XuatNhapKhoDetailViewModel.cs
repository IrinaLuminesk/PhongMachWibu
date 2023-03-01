using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Report
{
    public class XuatNhapKhoDetailViewModel
    {

        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string Unit { get; set; }
        public decimal? ImportTotal { get; set; }

        public string Provider { get; set; }

        public DateTime? Importdate { get; set; }
        public decimal? Price { get; set; }
    }
}
