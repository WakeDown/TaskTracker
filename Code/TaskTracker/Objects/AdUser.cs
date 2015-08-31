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
    }
}