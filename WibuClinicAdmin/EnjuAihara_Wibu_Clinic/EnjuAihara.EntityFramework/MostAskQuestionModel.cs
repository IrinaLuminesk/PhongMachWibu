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
    
    public partial class MostAskQuestionModel
    {
        public System.Guid MostAskQuestionId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<bool> Actived { get; set; }
        public Nullable<int> OrderIndex { get; set; }
    
        public virtual AccountModel AccountModel { get; set; }
    }
}
