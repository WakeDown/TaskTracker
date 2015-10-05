using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskPlan
    {
        public int TaskPlanId { get; set; }
        public int TaskClaimId { get; set; }
        public virtual TaskClaim TaskClaim { get; set; }
        public int TaskCheckpointId { get; set; }
        public virtual TaskCheckpoint TaskCheckpoint { get; set; }
        public DateTime PlanDate { get; set; }
        public bool Enabled { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatorSid { get; set; }
        public DateTime DateDelete { get; set; }
        public string DeleterSid { get; set; }

        public static async Task<IEnumerable<TaskPlan>> GetListAsync(AdUser curUser, DateTime planDate)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var list = await db.TaskPlans.Where(x => x.Enabled && x.PlanDate.Date == planDate.Date).ToListAsync();
            return list;
        }
    }
}