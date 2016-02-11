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
    public class ProjectModel
    {
        //[Key]
        //public int ProjectId { get; set; }
        //public string Name { get; set; }
        //[NotMapped]
        //public bool Selected { get; set; }
        //public string BgColor { get; set; }
        //public string ManagerSid { get; set; }
        //public string SysName { get; set; }

        //public Project()
        //{
        //}

        //public Project(string name)
        //{
        //    Name = name;
        //}

        public static IEnumerable<Project> GetList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var list = db.Projects.OrderBy(c => c.Name).ToList();
           
            //if (list.Any())
            //{
            //    list.First().Selected = true;
            //}
            return list;
        }

        public static SelectList GetSelectionList(int? id = null)
        {

            var list = GetList().ToList();
            var selList = new SelectList(list, "ProjectId", "Name", id);
            return selList;
        }

        public static int GetBySysName(string sysName)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.Projects.Where(c => c.SysName == sysName).Single().ProjectId;
        }
    }
}