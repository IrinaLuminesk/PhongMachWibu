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
    
    public partial class CatalogTypeModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CatalogTypeModel()
        {
            this.CatalogModels = new HashSet<CatalogModel>();
        }
    
        public string CatalogTypeCode { get; set; }
        public string CatalogTypeName { get; set; }
        public Nullable<bool> Actived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatalogModel> CatalogModels { get; set; }
    }
}