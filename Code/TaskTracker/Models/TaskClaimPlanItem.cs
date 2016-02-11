using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTracker.Models
{
    public class TaskClaimPlanItem
    {
        public TaskClaim TaskClaim { get; set; }
        public IEnumerable<TaskCheckpoint> Checkpoints { get; set; }
        public IEnumerable<TaskClaim> TaskClaimChilds { get; set; }
    }
}