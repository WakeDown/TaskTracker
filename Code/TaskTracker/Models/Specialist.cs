using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    
    public class Specialist
    {
        [Key]
        public string SpecialistSid { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        [MaxLength(150)]
        public string Email { get; set; }

        //public string SpecialistCategorySysName { get; set; }
        //public virtual SpecialistCategory SpecialistCategory { get; set; }

        public static IEnumerable<KeyValuePair<string, string>> GetProgrammers()
        {
            var list = AdHelper.GetSpecialistList(AdGroup.TaskTrackerProg);
            return list;
        }

        public static SelectList GetProgrammersSelectionList()
        {
            return new SelectList(GetProgrammers(), "Key", "Value");
        }
    }
}