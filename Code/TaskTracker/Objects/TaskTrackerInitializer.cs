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
        //protected override void Seed(TaskTrackerContext context)
        //{
        //    base.Seed(context);
            
        //    var tCats = new List<TaskCategory>();
        //    tCats.Add(new TaskCategory(1, "Feature", 2, "FEATURE") { Selected = true });
        //    tCats.Add(new TaskCategory(2, "Bug", 1, "BUG"));
        //    tCats.ForEach(s => context.TaskCategories.Add(s));
        //    context.SaveChanges();

        //    var tImps = new List<TaskImportant>();
        //    tImps.Add(new TaskImportant("Важно", 1, "IMP"));
        //    tImps.Add(new TaskImportant("Не важно", 2, "NOTIMP"));
        //    tImps.ForEach(s => context.TaskImportants.Add(s));
        //    context.SaveChanges();

        //    var tQuicks = new List<TaskQuickly>();
        //    tQuicks.Add(new TaskQuickly("Срочно", 1, "QUICK"));
        //    tQuicks.Add(new TaskQuickly("Не срочно", 2, "NOTQUICK"));
        //    tQuicks.ForEach(s => context.TaskQuicklies.Add(s));
        //    context.SaveChanges();

        //    //var specCats = new List<SpecialistCategory>();
        //    //specCats.Add(new SpecialistCategory() { Name = "Менеджер", SpecialistCategorySysName = "MGR" });
        //    //specCats.Add(new SpecialistCategory() { Name = "Программист", SpecialistCategorySysName = "PROG" });
        //    //specCats.ForEach(s => context.SpecialistCategories.Add(s));
        //    //context.SaveChanges();

        //    var tStates = new List<TaskState>();
        //    tStates.Add(new TaskState("Создано", "NEW", "red lighten-2", 1));
        //    tStates.Add(new TaskState("Назначено", "SETED", "orange", 2));
        //    tStates.Add(new TaskState("В работе", "WORK", "orange", 3));
        //    tStates.Add(new TaskState("Приостановлено", "PAUSED", "blue", 4));
        //    tStates.Add(new TaskState("На проверке", "TEST", "amber", 5));
        //    tStates.Add(new TaskState("Доработка", "WORK2", "orange", 6));
        //    tStates.Add(new TaskState("Проверено", "DONE", "green", 7));
        //    tStates.ForEach(s => context.TaskStates.Add(s));
        //    context.SaveChanges();

        //    var specs = AdHelper.GetSpecialistListS(AdGroup.TaskTrackerProg).ToList();
        //    specs.AddRange(AdHelper.GetSpecialistListS(AdGroup.TaskTrackerManager));
        //    specs.GroupBy(x => x.SpecialistSid).Select(s=> new Specialist { SpecialistSid= s.First().SpecialistSid, DisplayName = s.First().DisplayName, FullName = s.First().FullName }).ForEach(s => context.Specialists.Add(s));
        //    context.SaveChanges();

        //    var proj = new List<Project>();
        //    proj.Add(new Project("Сервис") {ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("Заявки на ЗИП") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("ДСУ планирование") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("СпецРасчет") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("Портал") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("TaskTracker") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("API") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("Эталон") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("1С") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.Add(new Project("UN1T счетчик") { ManagerSid = "S-1-5-21-1970802976-3466419101-4042325969-2365" });
        //    proj.ForEach(s => context.Projects.Add(s));
        //    context.SaveChanges();
        //}

    }
}