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
    
    public partial class TaskWork
    {
        public int Id { get; set; }
        public System.DateTime DateWork { get; set; }
        public string Name { get; set; }
        public decimal Hours { get; set; }
        public string CreatorSid { get; set; }
        public System.DateTime DateCreate { get; set; }
        public bool Enabled { get; set; }
        public int TaskId { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> DateClose { get; set; }
        public string CloserSid { get; set; }
        public string CloserName { get; set; }
    }
}
