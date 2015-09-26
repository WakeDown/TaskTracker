using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskTracker.Models
{
    public class Task2TaskState
    {
        [Key]
        public int TaskStateHistoryId { get; set; }
        public int TaskId { get; set; }
        public int TaskStateId { get; set; }
        public virtual TaskState TaskState { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatorSid { get; set; }
        public string Descr { get; set; }

        public Task2TaskState()
        {
        }
        public Task2TaskState(int taskId, int taskStateId, string creatorSid, string descr = null)
        {
            TaskId = taskId;
            TaskStateId = taskStateId;
            DateCreate=DateTime.Now;
            CreatorSid = creatorSid;
            Descr = descr;
        }
    }
}