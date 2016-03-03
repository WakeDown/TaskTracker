using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class WishState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }

        public WishState() { }

        public static async Task<WishState> Get(string sysName)
        {
            using (var db = new TaskTrackerContext())
            {
                var state= await db.WishStates.Where(x => x.SysName == sysName).SingleAsync();
                return state;
            }
        }
    }
}