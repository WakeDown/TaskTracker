using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskImportant
    {
        [Key]
        public int TaskImportantId { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public int OrderNum { get; set; }
        [NotMapped]
        public bool Selected { get; set; }

        public TaskImportant()
        {
        }

        public TaskImportant(string name, int orderNum, string sysName)
        {
            Name = name;
            OrderNum = orderNum;
            SysName = sysName;
        }

        public static IEnumerable<TaskImportant> GetList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var list = db.TaskImportants.OrderBy(c => c.OrderNum).ThenBy(c => c.Name).ToList();

            if (list.Any())
            {
                list.First().Selected = true;
            }
            return list;
        }

        public static SelectList GetSelectionList()
        {
            var list = GetList().ToList();
            return new SelectList(list, "TaskImportantId", "Name");
        }
    }
}