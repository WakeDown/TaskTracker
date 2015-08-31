using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskState
    {
        [Key]
        public int TaskStateId { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public int OrderNum { get; set; }
        public string BgColor { get; set; }

        public TaskState() { }
        public TaskState(string name, string sysName, string bgColor, int orderNum)
        {
            Name = name;
            SysName = sysName;
            OrderNum = orderNum;
        }

        public static TaskState GetFirstState()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskStates.First(x => x.SysName.Equals("NEW"));
        }
    }
}