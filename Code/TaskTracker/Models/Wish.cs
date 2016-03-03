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
        public string StateChangerSid { get; set; }
        public DateTime? StateChangeDate { get; set; }
        public int? StateId { get; set; }
        public string StateDescr { get; set; }

        public static void Create(Wish wish, AdUser user)
        {
            using (var db = new TaskTrackerContext())
            {
                wish.CreateDate=DateTime.Now;
                wish.CreatorSid = user.Sid;
                wish.StateId = WishState.Get("NEW").Id;
                db.Wishes.Add(wish);
                db.SaveChanges();
            }
        }

        public async static Task<IEnumerable<wish_view>> GetList(AdUser user)
        {
            
                string creatorSid = user.Sid;
                if (user.HasAccess(AdGroup.TaskTrackerAdmin, AdGroup.TaskTrackerManager, AdGroup.TaskTrackerProg))
                    creatorSid = null;

                

            using (var db = new TaskTrackerEntitity())
            {
                using (var db2 = new TaskTrackerContext())
                {
                    var projIds = db2.Projects.Where(y => y.OwnerSid == user.Sid).Select(y => y.ProjectId).ToList();
                
                return
                        await
                            db.wish_view.Where(
                                x => (creatorSid == null || (creatorSid != null && x.CreatorSid == creatorSid)) || (projIds.Contains(x.ProjectId)))
                                .ToListAsync();
                }
            }
        }

        public async static Task SetState(int wishId, string stateSysName, string stateDescr, AdUser user)
        {
            using (var db = new TaskTrackerContext())
            {
                var wish = db.Wishes.Single(x => x.Id == wishId);
                wish.StateChangeDate = DateTime.Now;
                wish.StateChangerSid = user.Sid;
                wish.StateDescr = stateDescr;
                wish.StateId = (await WishState.Get(stateSysName)).Id;
                db.SaveChanges();
            }
        }
    }
}