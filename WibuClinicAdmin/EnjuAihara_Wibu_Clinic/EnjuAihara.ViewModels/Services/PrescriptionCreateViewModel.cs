using System;
using System.Collections.Generic;

namespace EnjuAihara.ViewModels.Services
{
    public class PrescriptionCreateViewModel
    {
        public Guid? FastSearch { get; set; }

        public bool PatientType { get; set; }

        public string Note { get; set; }

        public string PatientFirstName { get; set; }

        public string PatientLastName { get; set; }

        public string PatientPhone { get; set; }

        public int? DayOfMedicine { get; set; }

        public List<Guid> IllnessLst { get; set; }
    }
}
