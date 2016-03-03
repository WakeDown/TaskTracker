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
    public class TaskFile
    {
        public System.Guid TaskFileId { get; set; }
        public int TaskClaimId { get; set; }
        public virtual TaskClaim TaskClaim { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatorSid { get; set; }
        public bool Enabled { get; set; }
        public DateTime? DateDelete { get; set; }
        public string DeleterrSid { get; set; }

        public TaskFile()
        {
        }

        public async Task<System.Guid> Add(string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            TaskFileId = Guid.NewGuid();
            Enabled = true;
            CreatorSid = creatorSid;
            DateCreate = DateTime.Now;
            db.TaskFiles.Add(this);
            await db.SaveChangesAsync();
            return TaskFileId;
        }

        public static async Task<TaskFile> GetAsync(string guid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            Guid taskFileId = Guid.Parse(guid);
            var list = await db.TaskFiles.SingleAsync(x => x.TaskFileId== taskFileId);
            return list;
        }
        public static async Task<IEnumerable<TaskFile>> GetListAsync(int taskId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var list = await db.TaskFiles.Where(x => x.TaskClaimId == taskId && x.Enabled).ToListAsync();
            return list;
        }
    }
}