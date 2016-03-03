using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class Wish
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public string Where { get; set; }
        public string ActionsBefore { get; set; }
        public string Link { get; set; }
        public string CreatorSid { get; set; }
        public DateTime CreateDate { get; set; }

        public static void Create(Wish wish, AdUser user)
        {
            using (var db = new TaskTrackerContext())
            {
                wish.CreateDate=DateTime.Now;
                wish.CreatorSid = user.Sid;
                db.Wishes.Add(wish);
                db.SaveChanges();
            }
        }

        public async static Task<IEnumerable<Wish>> GetList(AdUser user)
        {
            using (var db = new TaskTrackerContext())
            {
                string creatorSid = user.Sid;
                if (user.HasAccess(AdGroup.TaskTrackerAdmin, AdGroup.TaskTrackerManager, AdGroup.TaskTrackerProg))
                    creatorSid = null;
               return await db.Wishes.Where(x=> creatorSid == null || (creatorSid != null && x.CreatorSid == creatorSid)).ToListAsync();
            }
        }
    }
}