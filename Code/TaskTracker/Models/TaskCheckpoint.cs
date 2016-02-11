using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskCheckpointModel
    {
        //[Key]
        //public int TaskCheckpointId { get; set; }
        //public int TaskId { get; set; }
        //public virtual TaskClaimModel TaskClaim { get; set; }
        //[MaxLength(500)]
        //public string Name { get; set; }
        //public bool Done { get; set; }
        //[MaxLength(46)]
        //public string CreatorSid { get; set; }
        //public DateTime DateCreate { get; set; }
        //public DateTime? DateDone { get; set; }
        //[MaxLength(46)]
        //public string DonerSid { get; set; }
        //public bool Enabled { get; set; }
        //public string DeleterSid { get; set; }
        //public DateTime? DateDelete { get; set; }
        //public int OrderNum { get; set; }
        //public DateTime? DateUndone { get; set; }
        //[MaxLength(46)]
        //public string UndonerSid { get; set; }
        //public int? Hours { get; set; }

        //public TaskCheckpoint()
        //{
        //}

        //public TaskCheckpoint(int taskId)
        //{
        //    TaskId = taskId;
        //}

        public static async Task<TaskCheckpoint> GetAsync(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskCheckpoints.SingleOrDefaultAsync(x => x.TaskCheckpointId == id);
        }

        public static TaskCheckpoint Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskCheckpoints.SingleOrDefault(x => x.TaskCheckpointId == id);
        }

        public static async Task SaveInfo(int id, int? hours)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var chkp = db.TaskCheckpoints.Single(x => x.TaskCheckpointId == id);
            chkp.Hours = hours;
            await db.SaveChangesAsync();
        }

        public static async Task<IEnumerable<TaskCheckpoint>> GetListAsync(int taskId, bool? done = null)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var list = db.TaskCheckpoints.Where(x => x.Enabled && x.TaskId == taskId && (!done.HasValue || (done.HasValue && x.Done== done))).OrderBy(x => x.OrderNum).ToListAsync();
                return await list;
        }

        public static async Task<IEnumerable<TaskCheckpoint>> GetActiveChkpListAsync(int taskId)
        {
            return await GetListAsync(taskId, false);
        }

        public async Task<int> AddAsync(string creatorSid, TaskCheckpoint chk)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            chk.Enabled = true;
            chk.CreatorSid = creatorSid;
            chk.DateCreate = DateTime.Now;
            db.TaskCheckpoints.Add(chk);
            await db.SaveChangesAsync();
            return chk.TaskCheckpointId;
        }

        public static void SetDone(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = db.TaskCheckpoints.Single(x => x.TaskCheckpointId == id);
            checkpoint.Done = true;
            checkpoint.DonerSid = creatorSid;
            checkpoint.DateDone = DateTime.Now;
            db.SaveChanges();
        }

        public static async Task SetDoneAsync(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = await db.TaskCheckpoints.SingleAsync(x => x.TaskCheckpointId == id);
            checkpoint.Done = true;
            checkpoint.DonerSid = creatorSid;
            checkpoint.DateDone = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public static void SetUndone(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = db.TaskCheckpoints.Single(x => x.TaskCheckpointId == id);
            checkpoint.Done = false;
            checkpoint.UndonerSid = creatorSid;
            checkpoint.DateUndone = DateTime.Now;
            db.SaveChanges();
        }

        public static async Task SetUndoneAsync(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = await db.TaskCheckpoints.SingleAsync(x => x.TaskCheckpointId == id);
            checkpoint.Done = false;
            checkpoint.UndonerSid = creatorSid;
            checkpoint.DateUndone = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public static async Task CloseAsync(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = await db.TaskCheckpoints.SingleAsync(x => x.TaskCheckpointId == id);
            checkpoint.Enabled = false;
            checkpoint.DeleterSid = creatorSid;
            checkpoint.DateDelete = DateTime.Now;
            await db.SaveChangesAsync();
        }
    }
}