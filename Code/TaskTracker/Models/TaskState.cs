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
    public class TaskState
    {
        [Key]
        public int TaskStateId { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public int OrderNum { get; set; }
        public string BgColor { get; set; }
        public bool ManagerSelectDefault { get; set; }
        public bool ProgSelectDefault { get; set; }
        public bool UserSelectDefault { get; set; }

        public TaskState() { }
        public TaskState(string name, string sysName, string bgColor, int orderNum)
        {
            Name = name;
            SysName = sysName;
            OrderNum = orderNum;
            BgColor = bgColor;
        }

        public async static Task<TaskState> Get(string sysName)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            //if (db.TaskStates.Any(x => x.SysName.ToUpper().Equals(sysName)))
            //{
                return await db.TaskStates.SingleOrDefaultAsync(x => x.SysName.Equals(sysName));
            //}
            //return new TaskState();
        }

        public static IEnumerable<TaskState> GetList()
        {

            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskStates;
        }

        public static IEnumerable<TaskState> GetManagerDefaultList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskStates.Where(x=>x.ManagerSelectDefault);
        }

        public static IEnumerable<TaskState> GetProgDefaultList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskStates.Where(x => x.ProgSelectDefault);
        }

        public static IEnumerable<TaskState> GetUserDefaultList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskStates.Where(x => x.UserSelectDefault);
        }

        public async static Task<TaskState> GetFirstState()
        {
            return await Get("NEW");
        }

        public async static Task<TaskState> GetTestState()
        {
            return await Get("TEST");
        }

        public async static Task<TaskState> GetDoneState()
        {
            return await Get("DONE");
        }

        public async static Task<TaskState> GetPauseState()
        {
            return await Get("PAUSE");
        }
        public async static Task<TaskState> GetSetedState()
        {
            return await Get("SETED");
        }
    }
}