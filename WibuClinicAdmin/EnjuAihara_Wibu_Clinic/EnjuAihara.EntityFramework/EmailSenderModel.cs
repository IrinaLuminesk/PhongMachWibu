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
    
    public partial class EmailSenderModel
    {
        public System.Guid EmailId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SendFrom { get; set; }
        public string SendTo { get; set; }
        public Nullable<bool> IsSend { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}