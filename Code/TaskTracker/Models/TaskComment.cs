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
    public class TaskComment
    {
        [Key]
        public int TaskCommentId { get; set; }
        public string Text { get; set; }
        public int TaskClaimId { get; set; }
        public virtual TaskClaim TaskClaim { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatorSid { get; set; }
        public bool Enabled { get; set; }
        public DateTime DateDelete { get; set; }
        public string DeleterSid { get; set; }

        public TaskComment()
        {
            
        }

        public static async Task<TaskComment> GetAsync(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskComments.SingleOrDefaultAsync(x => x.TaskCommentId == id);
        }

        public static TaskComment Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskComments.SingleOrDefault(x => x.TaskCommentId == id);
        }
        
        public static async Task<IEnumerable<TaskComment>> GetListAsync(int taskId)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var list = db.TaskComments.Where(x => x.Enabled && x.TaskClaimId == taskId).OrderBy(x => x.DateCreate).ToListAsync();
            return await list;
        }

        public async Task<int> AddAsync(string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            Enabled = true;
            CreatorSid = creatorSid;
            DateCreate = DateTime.Now;
            db.TaskComments.Add(this);
            await db.SaveChangesAsync();
            return TaskCommentId;
        }

        public static async Task CloseAsync(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = await db.TaskComments.SingleAsync(x => x.TaskCommentId == id);
            checkpoint.Enabled = false;
            checkpoint.DeleterSid = creatorSid;
            checkpoint.DateDelete = DateTime.Now;
            await db.SaveChangesAsync();
        }
    }
}