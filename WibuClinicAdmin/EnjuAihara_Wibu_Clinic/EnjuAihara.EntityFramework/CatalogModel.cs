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
    
    public partial class CatalogModel
    {
        public System.Guid CatalogId { get; set; }
        public string CatalogTypeCode { get; set; }
        public string CatalogCode { get; set; }
        public string CatalogDescription { get; set; }
        public int OrderIndex { get; set; }
        public Nullable<bool> Actived { get; set; }
    
        public virtual CatalogTypeModel CatalogTypeModel { get; set; }
    }
}
