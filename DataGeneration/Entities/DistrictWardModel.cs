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
    
    public partial class DistrictWardModel
    {
        public System.Guid DistrictWardId { get; set; }
        public Nullable<System.Guid> DistrictId { get; set; }
        public Nullable<System.Guid> WardId { get; set; }
    
        public virtual DistrictModel DistrictModel { get; set; }
        public virtual WardModel WardModel { get; set; }
    }
}