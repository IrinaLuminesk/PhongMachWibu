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
    
    public partial class PagePermissionModel
    {
        public Nullable<System.Guid> RoleId { get; set; }
        public Nullable<System.Guid> PageId { get; set; }
        public string FuntionId { get; set; }
        public System.Guid PagePermissionId { get; set; }
    
        public virtual FunctionModel FunctionModel { get; set; }
        public virtual PageModel PageModel { get; set; }
        public virtual RolesModel RolesModel { get; set; }
    }
}
