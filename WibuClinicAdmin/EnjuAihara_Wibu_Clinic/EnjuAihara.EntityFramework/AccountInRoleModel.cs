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
    
    public partial class AccountInRoleModel
    {
        public Nullable<System.Guid> AccountId { get; set; }
        public Nullable<System.Guid> RoleId { get; set; }
        public System.Guid AccountRoleId { get; set; }
    
        public virtual AccountModel AccountModel { get; set; }
        public virtual RolesModel RolesModel { get; set; }
    }
}