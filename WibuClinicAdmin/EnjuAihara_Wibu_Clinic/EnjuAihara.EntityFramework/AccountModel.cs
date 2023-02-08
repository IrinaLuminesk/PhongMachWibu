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
    
    public partial class AccountModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AccountModel()
        {
            this.AccountModel1 = new HashSet<AccountModel>();
            this.AccountRecoveryTokenModels = new HashSet<AccountRecoveryTokenModel>();
            this.ChangeDataLogModels = new HashSet<ChangeDataLogModel>();
            this.DateModels = new HashSet<DateModel>();
            this.DateModels1 = new HashSet<DateModel>();
            this.DateModels2 = new HashSet<DateModel>();
            this.DescriptionModels = new HashSet<DescriptionModel>();
            this.RolesModels = new HashSet<RolesModel>();
        }
    
        public System.Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public Nullable<System.Guid> RoleId { get; set; }
        public string ImagePath { get; set; }
        public Nullable<bool> Actived { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountModel> AccountModel1 { get; set; }
        public virtual AccountModel AccountModel2 { get; set; }
        public virtual UsersModel UsersModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountRecoveryTokenModel> AccountRecoveryTokenModels { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChangeDataLogModel> ChangeDataLogModels { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DateModel> DateModels { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DateModel> DateModels1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DateModel> DateModels2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DescriptionModel> DescriptionModels { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RolesModel> RolesModels { get; set; }
    }
}
