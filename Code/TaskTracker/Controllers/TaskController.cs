using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Models;
using TaskTracker.Objects;

namespace TaskTracker.Controllers
{
    public class TaskController : BaseController
    {
        // GET: Task
        public ActionResult List()
        {
            var list = Task.GetList();
            return View(list);
        }

        [HttpGet]
        public ActionResult New()
        {
            var task = new Task();
            return View(task);
        }

        [HttpPost]
        public ActionResult New(Task model)
        {
            model.Add(CurUser.Sid);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var task = Task.Get(id.Value);
            return View(task);
        }

        [HttpGet]
        public ActionResult Card(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var task = Task.Get(id.Value);
            return View(task);
        }
    }
}