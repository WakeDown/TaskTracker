using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskWork
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public decimal? Hours { get; set; }
        public DateTime DateCreate { get; set; }
        public bool Enabled { get; set; }
        public string CreatorSid { get; set; }
        public string CreatorName { get; set; }
        public DateTime DateWork { get; set; }
        public DateTime? DateClose { get; set; }
        public string CloserSid { get; set; }
        public string CloserName { get; set; }

        public static async Task<ListResult<TaskWork>> GetListAsync(int taskId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var q = from x in db.TaskWorks
                    where x.TaskId == taskId && x.Enabled
                    select x;

            var result = new ListResult<TaskWork>
            {
                Total = q.Count(),
                List = await q.OrderByDescending(x => x.Id).ToListAsync()
            };
            return result;
        }

        public static async Task<TaskWork> Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskWorks.SingleAsync(x => x.Id == id);
        }

        public async Task CreateAsync(AdUser user)
        {
            CreatorSid = user.Sid;
            CreatorName = user.ShortName;
            Enabled = true;
            DateCreate = DateTime.Now;

            TaskTrackerContext db = new TaskTrackerContext();
            db.TaskWorks.Add(this);
            await db.SaveChangesAsync();
        }

        public static async Task Close(int id, AdUser user)
        {

            TaskTrackerContext db = new TaskTrackerContext();
            var work = await db.TaskWorks.SingleAsync(x => x.Id == id);
            work.Enabled = false;
            work.DateClose = DateTime.Now;
            work.CloserSid = user.Sid;
            work.CloserName = user.ShortName;
            await db.SaveChangesAsync();
        }
    }
}