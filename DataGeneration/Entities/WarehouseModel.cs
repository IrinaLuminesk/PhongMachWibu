//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataGeneration.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class WarehouseModel
    {
        public System.Guid WarehouseId { get; set; }
        public Nullable<System.Guid> MedicineProviderId { get; set; }
        public Nullable<decimal> BoughtQuantity { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
        public Nullable<double> BoughtPrice { get; set; }
        public Nullable<decimal> InstockQuantity { get; set; }
        public Nullable<System.DateTime> BoughtDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<double> SalePrice { get; set; }
        public Nullable<double> SalePercentage { get; set; }
        public string ImportCode { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual AccountModel AccountModel { get; set; }
        public virtual MedicineProvideModel MedicineProvideModel { get; set; }
    }
}
