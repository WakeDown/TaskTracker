using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Models;
using TaskTracker.Objects;

namespace TaskTracker.Controllers
{
    public class PlaningController : BaseController
    {
        // GET: Planing
        public async Task<ActionResult> Index()
        {
            var list = new List<TaskClaimPlanItem>();
            var tasks = await TaskClaim.GetListAsync(CurUser);
            foreach (var task in tasks)
            {
                var chkp = await TaskCheckpoint.GetListAsync(task.TaskId);
                var item = new TaskClaimPlanItem() {TaskClaim = task, Checkpoints = chkp};
                list.Add(item);
            }

            

            return View(list);
        }

        [HttpPost]
        public async Task<JsonResult> GetPlanList(DateTime planDate, int[] selTasks, int[] selChkps)
        {
            //if (!planDate.HasValue) return Json(new {});

            var list = await TaskPlan.GetListAsync(CurUser, planDate);
            return Json(list);
        }
    }
}