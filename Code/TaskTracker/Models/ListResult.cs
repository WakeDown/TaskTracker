using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTracker.Models
{
    public class ListResult<T>
    {
        private IEnumerable<T> _list;
        public IEnumerable<T> List
        {
            get { return _list; }
            set
            {
                _list = value;
                ListCount = List.Count();
            }
        }

        public int Total { get; set; }
        public int ListCount { get; private set; }
    }
}