using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskSpecification
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public decimal? Cost { get; set; }
        public int? Quantity { get; set; }
        public int? QuantityTypeId { get; set; }
        public virtual QuantityType QuantityType { get; set; }
        public DateTime DateCreate { get; set; }
        public bool Enabled { get; set; }

        //public static async Task<ListResult<TaskSpecification>> GetListAsync(int taskId)
        public static async Task<IEnumerable<TaskSpecification>> GetListAsync(int taskId)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskSpecifications.Where(x => x.TaskId == taskId).OrderByDescending(x => x.Id).ToListAsync();
                //var q = from x in db.TaskSpecification
                //        where x.TaskId == taskId && x.Enabled
                //        select x;

                //var result = new ListResult<TaskSpecification>
                //{
                //    Total = q.Count(),
                //    List = await q.OrderByDescending(x => x.Id).ToListAsync()
                //};
                //return result;
        }

        public static async Task<TaskSpecification> Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskSpecifications.SingleAsync(x => x.Id == id);
        }
    }
}