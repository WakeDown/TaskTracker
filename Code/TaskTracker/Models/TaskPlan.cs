using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;
using WebGrease.Css.Extensions;

namespace TaskTracker.Models
{
    public class TaskPlanModel
    {
        //[Key]
        //public int TaskPlanId { get; set; }
        //public int TaskId { get; set; }
        //public virtual TaskClaimModel TaskClaim { get; set; }
        //public int? TaskCheckpointId { get; set; }
        //public virtual TaskCheckpoint TaskCheckpoint { get; set; }
        //public DateTime PlanDate { get; set; }
        //public bool Enabled { get; set; }
        //public DateTime DateCreate { get; set; }
        //public string CreatorSid { get; set; }
        //public DateTime? DateDelete { get; set; }
        //public string DeleterSid { get; set; }

        public static TaskPlan Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskPlans.Single(x => x.TaskPlanId == id);
        }

        public static async Task AddRange(string creatorSid,DateTime planDate, int[] taskIds, int[] checkpointIds)
        {
            if (taskIds == null && checkpointIds == null) throw new ArgumentException("Не указаны задачи и контрольные точки для сохранения");

            TaskTrackerContext db = new TaskTrackerContext();
            var taskPlanList = new List<TaskPlan>();
            if (taskIds != null && taskIds.Any())
            {
                foreach (int taskId in taskIds)
                {

                    var taskPlan = new TaskPlan()
                    {
                        PlanDate = planDate,
                        TaskId = taskId,
                        Enabled = true,
                        CreatorSid = creatorSid,
                        DateCreate = DateTime.Now
                    };
                    taskPlanList.Add(taskPlan);
                }

                //taskPlanList = taskIds.Select(delegate(int taskId)
                //{
                    

                    
                //    return new TaskPlan()
                //    {
                //        PlanDate = planDate,
                //        TaskId = taskId,
                //        Enabled = true,
                //        CreatorSid = creatorSid,
                //        DateCreate = DateTime.Now
                //    };
                //}).ToList();
            }
            if (checkpointIds != null && checkpointIds.Any())
            {
                taskPlanList.AddRange(checkpointIds.Select(checkpointId => new TaskPlan()
                {
                    PlanDate = planDate,
                    TaskId = TaskCheckpoint.Get(checkpointId).TaskId,
                    TaskCheckpointId = checkpointId,
                    Enabled = true,
                    CreatorSid = creatorSid,
                    DateCreate = DateTime.Now
                }));
            }


            db.TaskPlans.AddRange(taskPlanList);
            await db.SaveChangesAsync();
        }

        public static async Task<IEnumerable<TaskPlan>> GetListAsync(AdUser curUser, DateTime planDate)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var list = await db.TaskPlans.Where(x => x.Enabled && DbFunctions.TruncateTime(x.PlanDate)==DbFunctions.TruncateTime(planDate)).ToListAsync();
            return list;
        }

        public static async Task<IEnumerable<int>> GetListIdsAsync(AdUser curUser, DateTime planDate)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var list = await db.TaskPlans.Where(x => x.Enabled && DbFunctions.TruncateTime(x.PlanDate) == DbFunctions.TruncateTime(planDate)).Select(x=>x.TaskPlanId).ToListAsync();
            return list;
        }

        public static void Close(string creatorSid, int taskPlanId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var taskPlan = db.TaskPlans.Single(x => x.TaskPlanId == taskPlanId);
            taskPlan.Enabled = false;
            taskPlan.DeleterSid = creatorSid;
            taskPlan.DateDelete = DateTime.Now;
            db.SaveChanges();
        }

        public static void Restore(string creatorSid, int taskPlanId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var taskPlan = db.TaskPlans.Single(x => x.TaskPlanId == taskPlanId);
            taskPlan.Enabled = true;
            taskPlan.DeleterSid = null;
            taskPlan.DateDelete =null;
            db.SaveChanges();
        }

        public static void Done(string creatorSid, int taskPlanId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var taskPlan = db.TaskPlans.Single(x => x.TaskPlanId == taskPlanId);

            //taskPlan.Enabled = true;
            //taskPlan.DeleterSid = creatorSid;
            //taskPlan.DateDelete = DateTime.Now;
            db.SaveChanges();
        }

        public static void Undone(string creatorSid, int taskPlanId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var taskPlan = db.TaskPlans.Single(x => x.TaskPlanId == taskPlanId);
            //taskPlan.Enabled = true;
            //taskPlan.DeleterSid = creatorSid;
            //taskPlan.DateDelete = DateTime.Now;
            db.SaveChanges();
        }
    }
}