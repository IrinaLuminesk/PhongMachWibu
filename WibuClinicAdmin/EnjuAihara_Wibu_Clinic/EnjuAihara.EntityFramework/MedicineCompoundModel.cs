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
    
    public partial class MedicineCompoundModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> IngredientId { get; set; }
        public Nullable<System.Guid> MedicineId { get; set; }
    
        public virtual IngredientModel IngredientModel { get; set; }
        public virtual MedicineModel MedicineModel { get; set; }
    }
}