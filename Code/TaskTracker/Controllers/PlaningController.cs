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
            var tasks = await TaskClaim.GetActiveTaskListAsync(CurUser);
            foreach (var task in tasks)
            {
                var chkp = await TaskCheckpoint.GetActiveChkpListAsync(task.TaskId);
                var item = new TaskClaimPlanItem() {TaskClaim = task, Checkpoints = chkp};
                list.Add(item);
            }
            
            return View(list);
        }

        public async Task<JsonResult> DeletePlanTask(int id)
        {
            return Json(new {});
        }

        public async Task<JsonResult> Add2Plan(DateTime planDate, int[] selTasks = null, int[] selChkps = null)
        {
            await TaskPlan.AddRange(CurUser.Sid, planDate,selTasks, selChkps);

            return Json(new {});
        }

        [HttpPost]
        public async Task<JsonResult> GetPlanList(DateTime planDate)
        {
            //if (!planDate.HasValue) return Json(new {});
            //var planDate = DateTime.Parse(planDateStr);
            var list = await TaskPlan.GetListIdsAsync(CurUser, planDate);
            return Json(list);
        }
        [HttpPost]
        public ActionResult GetTaskPlanItem(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var taskPlan = TaskPlan.Get(id.Value);
            return PartialView("TaskPlanItem", taskPlan);
        }
    }
}