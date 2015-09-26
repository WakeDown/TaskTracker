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
    public class TaskCategory
    {
        public TaskCategory() { }
        public TaskCategory(int id, string name, int orderNum, string sysName)
        {
            TaskCategoryId = id;
            Name = name;
            OrderNum = orderNum;
            SysName = sysName;
        }
        [Key]
        public int TaskCategoryId { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public string SysName { get; set; }
        [NotMapped]
        public bool Selected { get; set; }

        public static IEnumerable<TaskCategory> GetList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var list = db.TaskCategories.OrderBy(c => c.OrderNum).ThenBy(c => c.Name);
            if (list.Any())
            {
                list.First().Selected = true;
            }
            return list;
        } 

        public static SelectList GetSelectionList()
        {
            var list = GetList().ToList();
            return new SelectList(list, "TaskCategoryId", "Name", list.First(m => m.Selected).TaskCategoryId);
        }
    }
}