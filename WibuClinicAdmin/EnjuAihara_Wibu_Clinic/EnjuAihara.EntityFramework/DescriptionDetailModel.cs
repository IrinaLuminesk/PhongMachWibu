//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnjuAihara.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class DescriptionDetailModel
    {
        public System.Guid DescriptionDetailId { get; set; }
        public Nullable<System.Guid> DescriptionId { get; set; }
        public Nullable<System.Guid> MedicineId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
    
        public virtual DescriptionModel DescriptionModel { get; set; }
        public virtual WarehouseModel WarehouseModel { get; set; }
    }
}
