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
    
    public partial class AccountRecoveryTokenModel
    {
        public System.Guid TokenId { get; set; }
        public string Token { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
        public Nullable<System.Guid> AccountId { get; set; }
    
        public virtual AccountModel AccountModel { get; set; }
    }
}