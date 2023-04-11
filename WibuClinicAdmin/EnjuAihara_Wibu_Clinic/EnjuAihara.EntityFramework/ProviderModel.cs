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
    
    public partial class ProviderModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProviderModel()
        {
            this.MedicineProvideModels = new HashSet<MedicineProvideModel>();
        }
    
        public System.Guid ProviderId { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public Nullable<bool> Actived { get; set; }
        public string Address { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> longitude { get; set; }
        public Nullable<System.Guid> CityId { get; set; }
        public Nullable<System.Guid> DistrictId { get; set; }
    
        public virtual CityModel CityModel { get; set; }
        public virtual DistrictModel DistrictModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicineProvideModel> MedicineProvideModels { get; set; }
    }
}
