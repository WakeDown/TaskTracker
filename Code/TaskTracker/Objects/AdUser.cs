using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskTracker.Helpers;

namespace TaskTracker.Objects
{
    public class AdUser
    {
        public string Sid { get; set; }
        public string Login { get; set; }
        //public List<AdGroup> AdGroups { get; set; }
        private string _fullName;
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
                ShortName = StringHelper.ShortName(_fullName);
            }
        }

        public string Email { get; set; }
        public string ShortName { get; set; }

        public bool Is(params AdGroup[] groups)
        {
            bool result = false;

            if (String.IsNullOrEmpty(Sid)) return false;
            result = AdHelper.UserInGroup(Sid, groups);
            //foreach (AdGroup group in groups)
            //{
            //    result = AdGroups.Contains(group);
            //    if (result) break;
            //}
            return result;
        }

        public bool HasAccess(params AdGroup[] groups)
        {
            bool result = false;
            if (String.IsNullOrEmpty(Sid)) return false;
            if (AdHelper.UserInGroup(Sid, AdGroup.TaskTrackerAdmin)) return true;
            result = AdHelper.UserInGroup(Sid, groups);
            return result;
        }
    }
}