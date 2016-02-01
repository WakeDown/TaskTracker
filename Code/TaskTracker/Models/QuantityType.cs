using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class QuantityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }

        public static IEnumerable<QuantityType> GetList()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.QuantityTypes.OrderBy(x=>x.OrderNum);
        }
    }
}