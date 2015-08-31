using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TaskTracker.Models;
using WebGrease.Css.Extensions;

namespace TaskTracker.Objects
{
    public class TaskTrackerInitializer : DropCreateDatabaseIfModelChanges<TaskTrackerContext>
    {
        protected override void Seed(TaskTrackerContext context)
        {
            base.Seed(context);
            
            var tCats = new List<TaskCategory>();
            tCats.Add(new TaskCategory(1, "Feature", 1) { Selected = true });
            tCats.Add(new TaskCategory(2, "Bug", 2));
            tCats.ForEach(s => context.TaskCategories.Add(s));
            context.SaveChanges();

            //var specCats = new List<SpecialistCategory>();
            //specCats.Add(new SpecialistCategory() { Name = "Менеджер", SpecialistCategorySysName = "MGR" });
            //specCats.Add(new SpecialistCategory() { Name = "Программист", SpecialistCategorySysName = "PROG" });
            //specCats.ForEach(s => context.SpecialistCategories.Add(s));
            //context.SaveChanges();

            var tStates = new List<TaskState>();
            tStates.Add(new TaskState("Создано", "NEW", "yellow", 1));
            tStates.Add(new TaskState("Назначено", "SET","orange", 2));
            tStates.Add(new TaskState("Приостановлено", "PAUSED", "blue", 3));
            tStates.Add(new TaskState("Выполнено", "DONE", "green", 4));
            tStates.ForEach(s => context.TaskStates.Add(s));
            context.SaveChanges();

            var specs = AdHelper.GetSpecialistListS(AdGroup.TaskTrackerProg).ToList();
            specs.AddRange(AdHelper.GetSpecialistListS(AdGroup.TaskTrackerManager));
            specs.GroupBy(x => x.SpecialistSid).Select(s=> new Specialist { SpecialistSid= s.First().SpecialistSid, DisplayName = s.First().DisplayName, FullName = s.First().FullName }).ForEach(s => context.Specialists.Add(s));
            context.SaveChanges();
        }
    }
}