﻿using System;
using EnjuAihara.EntityFramework;

namespace EnjuAihara.ViewModels.Warehouse
{
    public class StockReceivingDetailViewModel:WarehouseDetailModel
    {
        public Guid? MedicineId { get; set; }
        public Guid? ProviderId { get; set; }
        public int STT { get; set; }
        public string MedicineName { get; set; }
        public string ProviderName { get; set; }
        public string MedicineCode { get; set; }
        public string ProviderCode { get; set; }
        public string ImportCode { get; set; }
        public Nullable<System.DateTime> BoughtDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }

    }
}
