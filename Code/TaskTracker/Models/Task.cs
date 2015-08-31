using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public int TaskCategoryId { get; set; }
        public virtual TaskCategory TaskCategory { get; set; }
        public bool Enabled { get; set; }

        public DateTime? DateStartPlan { get; set; }
        public DateTime? DateEndPlan { get; set; }
        public DateTime? DateStartFact { get; set; }
        public DateTime? DateEndFact { get; set; }

        public string SpecialistSid { get; set; }
        public virtual Specialist Specialist { get; set; }

        public int TaskStateId { get; set; }
        public TaskState TaskState { get; set; }
        public string CreatorSid { get; set; }

        public Task()
        {
            
        }

        public static Task Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            if (db.Tasks.Any(x => x.TaskId == id))
            {
                return db.Tasks.First(x => x.TaskId == id);
            }
            else
            {
                return new Task();
            }
        }

        public static IEnumerable<Task> GetList()
        {
            TaskTrackerContext db = new TaskTrackerContext();

            if (!db.Tasks.Any())
            {
                return new List<Task>();
            }
            if (db.Tasks.Any(x=>x.Enabled))
            //if (db.Tasks.Any())
            {
                var list = db.Tasks.Where(x => x.Enabled).OrderBy(x => x.DateStartPlan).ToList();
                //var list = db.Tasks.OrderBy(x => x.DateStartPlan);
                return list;
            }
            
                return new List<Task>();
            
            
        }

        public void Add(string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            Enabled = true;
            CreatorSid = creatorSid;
            var firstStateId = TaskState.GetFirstState().TaskStateId;
            TaskStateId = firstStateId;
            db.Tasks.Add(this);
            db.SaveChanges();
            var state = new Task2TaskState(TaskId, firstStateId, creatorSid);
            db.TaskStateHistory.Add(state);
            db.SaveChanges();
        }

        public IEnumerable<Task2TaskState> GetStateHistory()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskStateHistory.Where(x => x.TaskId == TaskId).OrderByDescending(x => x.DateCreate).ToList();
        }
    }
}