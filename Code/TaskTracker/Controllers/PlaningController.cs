using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Models;
using TaskTracker.Objects;

namespace TaskTracker.Controllers
{
    public class PlaningController : BaseController
    {
        // GET: Planing
        public ActionResult Index()
        {
            var list = new List<TaskClaimPlanItem>();
            return View(list);
        }
    }
}