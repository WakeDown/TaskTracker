using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTracker.Objects
{
    public class AdUserGroup
    {

        public AdGroup Group { get; set; }
        public string Sid { get; set; }
        public string Name { get; set; }

        public AdUserGroup(AdGroup grp, string sid, string name)
        {
            Group = grp;
            Sid = sid;
            Name = name;
        }

        public static IEnumerable<AdUserGroup> GetList()
        {
            var list = new List<AdUserGroup>();

            list.Add(new AdUserGroup(AdGroup.TaskTrackerAdmin, "S-1-5-21-1970802976-3466419101-4042325969-4090", "TaskTrackerAdmin"));
            list.Add(new AdUserGroup(AdGroup.TaskTrackerManager, "S-1-5-21-1970802976-3466419101-4042325969-4091", "TaskTrackerManager"));
            list.Add(new AdUserGroup(AdGroup.TaskTrackerProg, "S-1-5-21-1970802976-3466419101-4042325969-4089", "TaskTrackerProg"));

            return list;
        }


        public static string GetSidByAdGroup(AdGroup grp)
        {
            return GetList().Single(g => g.Group == grp).Sid;
        }

        public static AdGroup GetAdGroupBySid(string sid)
        {
            if (string.IsNullOrEmpty(sid)) return AdGroup.None;
            var grp = GetList().Single(g => g.Sid == sid).Group;
            return grp;
        }
    }
}