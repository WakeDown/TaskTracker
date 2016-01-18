using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task<ActionResult> List(string tsts, string spec = null, string author = null, string prjs = null)
        {
            string[] statesStr = String.IsNullOrEmpty(tsts) ? new string[0] : tsts.Split(',');
            int[] states = Array.ConvertAll<string, int>(statesStr, int.Parse);

            string[] projectsStr = String.IsNullOrEmpty(prjs) ? new string[0] : prjs.Split(',');
            int[] projects = Array.ConvertAll<string, int>(projectsStr, int.Parse);

            if (CurUser.Is(AdGroup.TaskTrackerManager))
            {
                //if (states == null || !states.Any())
                //{
                //    states = TaskState.GetManagerDefaultList().Select(x=>x.TaskStateId).ToArray();
                //}
                
                var list = await TaskClaim.GetListAsync(CurUser, spec, author, states, projects);
                list = list.OrderByDescending(x => x.DateCreate);
                ViewBag.AdGroup = AdGroup.TaskTrackerManager;
                return View("List", list);
            }
            else if (CurUser.Is(AdGroup.TaskTrackerProg))
            {
                //if (states == null || !states.Any())
                //{
                //    states = TaskState.GetProgDefaultList().Select(x => x.TaskStateId).ToArray();
                //}

                //spec = CurUser.Sid;
                var list = await TaskClaim.GetListAsync(CurUser, spec, author, states, projects);
                list = list.OrderByDescending(x => x.DateCreate);
                ViewBag.AdGroup = AdGroup.TaskTrackerProg;
                return View("List", list);
            }
            else
            {
                //if (states == null || !states.Any())
                //{
                //    states = TaskState.GetUserDefaultList().Select(x => x.TaskStateId).ToArray();
                //}

                //author = CurUser.Sid;
                var list = await TaskClaim.GetListAsync(CurUser, spec, author, states, projects);
                list = list.OrderByDescending(x => x.DateCreate);
                return View("List", list);
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
        public async Task<ActionResult> GetTaskFileData(string guid)
        {
            var file = await TaskFile.GetAsync(guid);
            return File(file.Data, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }

        [HttpGet]
        public async Task<ActionResult> New(int? idParent)
        {
            var task = new TaskClaim();

            if (idParent.HasValue)
            {
                var parent = await TaskClaim.GetAsync(idParent.Value);

                task = new TaskClaim() { ParentTaskId = idParent, ProjectId = parent .ProjectId};
                task.ParentTask = parent;
            }
            
            return View(task);
        }

        [HttpPost]
        public async Task<ActionResult> New(TaskClaim model)
        {
            try
            {
                int taskId = await model.Add(CurUser.Sid);
                await SaveFile2Task(taskId);
                //if (Request.Files.Count > 0)
                //{
                //    for (int i =0; i< Request.Files.Count ; i++)
                //    {
                //        var file = Request.Files[i];
                //        if (file != null && file.ContentLength > 0)
                //        {
                //            //string ext = Path.GetExtension(file.FileName).ToLower();
                //            //if (ext != ".png" && ext != ".jpeg" && ext != ".jpg" && ext != ".gif") throw new Exception("Формат фотографии должен быть .png .jpeg .gif");

                //            byte[] data = null;
                //            using (var br = new BinaryReader(file.InputStream))
                //            {
                //                data = br.ReadBytes(file.ContentLength);
                //            }
                //            var taskFile = new TaskFile() {TaskClaimId = taskId, Data = data, Name=file.FileName};
                //            await taskFile.Add(CurUser.Sid);
                //        }
                //    }
                //}

                if (!String.IsNullOrEmpty(Request.Form["Continue"]))
                {
                    return RedirectToAction("Card", new { id = taskId });
                }
                else
                {
                    return View("WindowClose");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("New", model);
            }

            return RedirectToAction("List");
        }



        [HttpPost]
        public async Task<ActionResult> AddFile2Task(int? taskId)
        {
            if (!taskId.HasValue) return HttpNotFound();
            await SaveFile2Task(taskId.Value);
            return RedirectToAction("Card", new {id = taskId.Value});
        }

        public async Task SaveFile2Task(int taskId)
        {
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        //string ext = Path.GetExtension(file.FileName).ToLower();
                        //if (ext != ".png" && ext != ".jpeg" && ext != ".jpg" && ext != ".gif") throw new Exception("Формат фотографии должен быть .png .jpeg .gif");

                        byte[] data = null;
                        using (var br = new BinaryReader(file.InputStream))
                        {
                            data = br.ReadBytes(file.ContentLength);
                        }
                        var taskFile = new TaskFile() { TaskClaimId = taskId, Data = data, Name = file.FileName };
                        await taskFile.Add(CurUser.Sid);
                    }
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.GetAsync(id.Value);
            return View(task);
        }

        [HttpGet]
        public async Task<ActionResult> Card(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.GetAsync(id.Value);
            ViewBag.StateHistory = await task.GetStateHistoryAsync();

            if (CurUser.Is(AdGroup.TaskTrackerManager, AdGroup.TaskTrackerProg))
            {
                ViewBag.Checkpoints = await TaskCheckpoint.GetListAsync(id.Value);
                ViewBag.TaskFiles = await TaskFile.GetListAsync(id.Value);
                ViewBag.TaskComments = await TaskComment.GetListAsync(id.Value);
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
            var chk = new TaskCheckpoint() { TaskId = taskId, Name = chekpointName};
            int id = await chk.AddAsync(CurUser.Sid);
            return Json(new {id= id });
        }

        [HttpPost]
        public async Task<JsonResult> AddTaskComment(int taskId, string commentText)
        {
            var chk = new TaskComment() { TaskClaimId = taskId, Text = commentText };
            int id = await chk.AddAsync(CurUser.Sid);
            return Json(new { id = id });
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
            var task = await TaskClaim.GetAsync(id.Value);
            ViewBag.TaskCategoryList = TaskCategory.GetList();
            ViewBag.TaskImportantList = TaskImportant.GetList();
            ViewBag.TaskQuicklyList = TaskQuickly.GetList();
            ViewBag.SpecialistList = Specialist.GetProgrammers();
            return PartialView("TaskListManagerItem", task);
        }

        [HttpGet]
        public async Task<ActionResult> GetTaskListProgItem(int? id)
        {
            //int tid = int.Parse(id);
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.GetAsync(id.Value);
            return PartialView("TaskListProgItem", task);
        }

        [HttpGet]
        public async Task<ActionResult> GetTaskListUserItem(int? id)
        {
            //int tid = int.Parse(id);
            if (!id.HasValue) return HttpNotFound();
            var task = await TaskClaim.GetAsync(id.Value);
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
        [HttpGet]
        public async Task<ActionResult> GetTaskCommentItem(int id)
        {
            var comment = await TaskComment.GetAsync(id);
            return PartialView("TaskCommentItem", comment);
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

        [HttpPost]
        public async Task<JsonResult> GetTaskCheckpointCount(int taskId)
        {
            var list = await TaskCheckpoint.GetListAsync(taskId);
            int all = list.Count();
            int done = list.Count(x => x.Done);
            int undone = list.Count(x => !x.Done);
            return Json(new { all = all, done = done , undone = undone });
        }
    }
}