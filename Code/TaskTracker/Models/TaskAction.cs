using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskAction
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public string CreatorSid { get; set; }
        public string CreatorName { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? RemindDate { get; set; }
        public string RemindText { get; set; }
        public bool Enabled { get; set; }

        public async Task SaveAsync(AdUser user)
        {
            DateCreate = DateTime.Now;
            Enabled = true;
            CreatorSid = user.Sid;
            CreatorName = user.ShortName;
            var db = new TaskTrackerContext();
            db.TaskActions.Add(this);
            await db.SaveChangesAsync();
        }
    }
}