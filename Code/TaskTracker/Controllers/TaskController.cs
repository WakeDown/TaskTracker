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
    public class TaskController : BaseController
    {
        public async Task<ActionResult> List(string tsts, string spec = null, string author = null)
        {
            string[] statesStr = String.IsNullOrEmpty(tsts) ? new string[0] : tsts.Split(',');
            int[] states = Array.ConvertAll<string, int>(statesStr, int.Parse);

            if (CurUser.Is(AdGroup.TaskTrackerManager))
            {
                if (states == null || !states.Any())
                {
                    states = TaskState.GetManagerDefaultList().Select(x=>x.TaskStateId).ToArray();
                }
                
                var list = await TaskClaim.GetListAsync(spec, author, states);
                list = list.OrderByDescending(x => x.DateCreate);
                return View("ListManager", list);
            }
            else if (CurUser.Is(AdGroup.TaskTrackerProg))
            {
                if (states == null || !states.Any())
                {
                    states = TaskState.GetProgDefaultList().Select(x => x.TaskStateId).ToArray();
                }

                spec = CurUser.Sid;
                var list = await TaskClaim.GetListAsync(spec, author, states);
                list = list.OrderByDescending(x => x.DateCreate);
                return View("ListProg", list);
            }
            else
            {
                if (states == null || !states.Any())
                {
                    states = TaskState.GetUserDefaultList().Select(x => x.TaskStateId).ToArray();
                }

                author = CurUser.Sid;
                var list = await TaskClaim.GetListAsync(spec, author, states);
                list = list.OrderByDescending(x => x.DateCreate);
                return View("ListUser", list);
            }
        }

        //public async Task<ActionResult> ListManager(string spec = null, string author = null)
        //{

        //}

        //public async Task<ActionResult> ListProg(string spec = null, string author = null)
        //{

        //    var list = await TaskClaim.GetList(spec, author);
        //    list = list.OrderByDescending(x => x.DateCreate);
        //    return View(list);
        //}

        //public async Task<ActionResult> ListUser(string spec = null, string author = null)
        //{

        //    var list = await TaskClaim.GetList(spec, author);
        //    list = list.OrderByDescending(x => x.DateCreate);
        //    return View(list);
        //}

        [HttpGet]
        public ActionResult New()
        {
            var task = new TaskClaim();
            return View(task);
        }

        [HttpPost]
        public async Task<ActionResult> New(TaskClaim model)
        {
            try
            {
                await model.Add(CurUser.Sid);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("New", model);
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.Get(id.Value);
            return View(task);
        }

        [HttpGet]
        public async Task<ActionResult> Card(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.Get(id.Value);
            ViewBag.StateHistory = await task.GetStateHistoryAsync();

            if (CurUser.Is(AdGroup.TaskTrackerManager, AdGroup.TaskTrackerProg))
            {
                ViewBag.Checkpoints = await TaskCheckpoint.GetListAsync(id.Value);
                return View("CardPerf",task);
            }
            else
            {
                return View("CardUser", task);
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddCheckpoint(int taskId, string chekpointName)
        {
            var chk = new TaskCheckpoint() {TaskClaimId = taskId, Name = chekpointName};
            int id = await chk.AddAsync(CurUser.Sid);
            return Json(new {id= id });
        }

        //[HttpPost]
        //public async Task<JsonResult> SetDiscardState(int id)
        //{
        //    await TaskClaim.SetTestState(id, CurUser.Sid);
        //    return Json(new { });
        //}

        [HttpPost]
        public async Task<JsonResult> SetDoneState(int id)
        {
            await TaskClaim.SetDone(id, CurUser.Sid);
            return Json(new { });
        }
        [HttpPost]
        public async Task<JsonResult> SetWorkState(int id)
        {
            await TaskClaim.SetWork(id, CurUser.Sid);
            return Json(new { });
        }
        [HttpPost]
        public async Task<JsonResult> SetPauseState(int id, string descr)
        {
            await TaskClaim.SetPause(id, descr, CurUser.Sid);
            return Json(new { });
        }

        [HttpPost]
        public async Task<JsonResult> SetReworkState(int id, string descr)
        {
            await TaskClaim.SetRework(id, descr, CurUser.Sid);
            return Json(new { });
        }
        

        //public async Task<ActionResult> StateHistory(int? id)
        //{
        //    if (!id.HasValue) return HttpNotFound();
        //    var list = await TaskClaim.GetStateHistory(id.Value);
        //    return View("StateHistory", list);
        //}

        [HttpGet]
        public async Task<ActionResult> GetTaskListManagerItem(int? id)
        {
            //int tid = int.Parse(id);
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.Get(id.Value);
            return PartialView("TaskListManagerItem", task);
        }

        [HttpGet]
        public async Task<ActionResult> GetTaskListProgItem(int? id)
        {
            //int tid = int.Parse(id);
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.Get(id.Value);
            return PartialView("TaskListProgItem", task);
        }

        [HttpGet]
        public async Task<ActionResult> GetTaskListUserItem(int? id)
        {
            //int tid = int.Parse(id);
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.Get(id.Value);
            return PartialView("TaskListUserItem", task);
        }
        //[HttpPost]
        //public async Task<JsonResult> GetTask(int? id)
        //{
        //    //int tid = int.Parse(id);
        //    if (!id.HasValue) return HttpNotFound();
        //    var task =  await TaskClaim.Get(id.Value);
        //    return Json(task);
        //}

        [HttpPost]
        public async Task<JsonResult> SetTaskCategory(int id, int cid)
        {
            await TaskClaim.SetCategory(id, cid, CurUser.Sid);
            return Json(new { });
        }

        [HttpPost]
        public async Task<JsonResult> SetTaskSpecialist(int id, string sid)
        {
            try
            {
                await TaskClaim.SetSpecialist(id, sid, CurUser.Sid);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            return Json(new { });
        }
        [HttpPost]
        public async Task<JsonResult> SetTaskImportant(int id, int iid)
        {
            await TaskClaim.SetTaskImportant(id, iid, CurUser.Sid);
            return Json(new { });
        }
        [HttpPost]
        public async Task<JsonResult> SetTaskQuickly(int id, int qid)
        {
            await TaskClaim.SetTaskQuickly(id, qid, CurUser.Sid);
            return Json(new { });
        }
        [HttpGet]
        public async Task<ActionResult> GetCheckpointItem(int id)
        {
            var chkp = await TaskCheckpoint.GetAsync(id);
            return PartialView("CheckpointItem", chkp);
        }
        [HttpPost]
        public async Task<JsonResult> SetCheckpointDone(int id)
        {
            await TaskCheckpoint.SetDoneAsync(id, CurUser.Sid);
            return Json(new { });
        }
        [HttpPost]
        public async Task<JsonResult> SetCheckpointUndone(int id)
        {
            await TaskCheckpoint.SetUndoneAsync(id, CurUser.Sid);
            return Json(new { });
        }

        //[HttpPost]
        //public JsonResult SetCheckpointDone(int id)
        //{
        //    TaskCheckpoint.SetDone(id, CurUser.Sid);
        //    return Json(new { });
        //}
        //[HttpPost]
        //public JsonResult SetCheckpointUndone(int id)
        //{
        //    TaskCheckpoint.SetUndone(id, CurUser.Sid);
        //    return Json(new { });
        //}

        [HttpPost]
        public async Task<JsonResult> CloseCheckpoint(int id)
        {
            await TaskCheckpoint.CloseAsync(id, CurUser.Sid);
            return Json(new { });
        }
    }
}