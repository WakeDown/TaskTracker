using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class WishModel
    {
        //public int Id { get; set; }
        //public int ProjectId { get; set; }
        //public string Title { get; set; }
        //public string Descr { get; set; }
        //public string Where { get; set; }
        //public string ActionsBefore { get; set; }
        //public string Link { get; set; }
        //public string CreatorSid { get; set; }
        //public DateTime CreateDate { get; set; }

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
    }
}