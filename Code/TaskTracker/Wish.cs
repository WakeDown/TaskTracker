//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskTracker
{
    using System;
    using System.Collections.Generic;
    
    public partial class Wish
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public string Where { get; set; }
        public string ActionsBefore { get; set; }
        public string Link { get; set; }
        public string CreatorSid { get; set; }
        public System.DateTime CreateDate { get; set; }
    
        public virtual Project Project { get; set; }
    }
}
