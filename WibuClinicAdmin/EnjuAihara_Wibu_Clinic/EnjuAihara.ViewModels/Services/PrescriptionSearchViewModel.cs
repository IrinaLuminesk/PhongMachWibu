﻿using System;
using System.Collections.Generic;

namespace EnjuAihara.ViewModels.Services
{
    public class PrescriptionSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public int STT { get; set; }
        public string PrescriptionCode { get; set; }

        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public List<string> MedicineList { get; set; }

        public List<string> IllnessList { get; set; }
        public string Note { get; set; }

        public Guid PrescriptionId { get; set; }
        public int Quantity { get; set; }
        public double? TotalMoney { get; set; }

        public string Status { get; set; }
        public bool? IsPay { get; set; }
    }
}
