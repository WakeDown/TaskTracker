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
    
    public partial class TaskQuickly
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaskQuickly()
        {
            this.TaskClaim = new HashSet<TaskClaim>();
        }
    
        public int TaskQuicklyId { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public string SysName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskClaim> TaskClaim { get; set; }
    }
}
