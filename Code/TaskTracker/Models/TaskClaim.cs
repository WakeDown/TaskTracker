using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using TaskTracker.Helpers;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskClaim
    {
        [Key]
        public int TaskId { get; set; }
        public int? ParentTaskId { get; set; }
        public virtual TaskClaim ParentTask { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public int? TaskCategoryId { get; set; }
        public virtual TaskCategory TaskCategory { get; set; }
        public bool Enabled { get; set; }

        public DateTime? DateStartPlan { get; set; }
        public DateTime? DateEndPlan { get; set; }
        public DateTime? DateStartFact { get; set; }
        public DateTime? DateEndFact { get; set; }
        public string SpecialistSid { get; set; }
        public virtual Specialist Specialist { get; set; }

        public int? TaskStateId { get; set; }
        public virtual TaskState TaskState { get; set; }
        public string ModifierSid { get; set; }
        public string CreatorSid { get; set; }
        public DateTime DateCreate { get; set; }
        public int? TaskImportantId { get; set; }
        public virtual TaskImportant TaskImportant { get; set; }
        public int? TaskQuicklyId { get; set; }
        public virtual TaskQuickly TaskQuickly { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int Rank { get; set; }
        public string RankChangerSid { get; set; }
        public DateTime? RankChangeDate { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Quantity { get; set; }
        public int? QuantityTypeId { get; set; }
        public virtual QuantityType QuantityType { get; set; }
        public bool NeedCheckpoints { get; set; }
        public bool NeedWorkList { get; set; }

        public TaskClaim()
        {

        }

        public static async Task SaveInfo(int id, decimal? cost, decimal? quantity, int? quantityTypeId, DateTime? DateStartPlan)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var task = db.Tasks.Single(x => x.TaskId == id);
            task.Cost = cost;
            task.Quantity = quantity;
            task.QuantityTypeId = quantityTypeId;
            task.DateStartPlan = DateStartPlan;
            await db.SaveChangesAsync();
        }

        public static async Task<TaskClaim> GetAsync(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.Tasks.SingleOrDefaultAsync(x => x.TaskId == id);
        }

        public static TaskClaim Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.Tasks.SingleOrDefault(x => x.TaskId == id);
        }

        public static async Task<IEnumerable<TaskClaim>> GetListAsync(AdUser curUser, string spec = null, string author = null, int[] states = null, int[] projects = null)
        {
            if (projects == null || !projects.Any())
            {
                projects = Project.GetList().Select(x=>x.ProjectId).ToArray();
            }

            if (curUser.Is(AdGroup.TaskTrackerManager))
            {
                if (states == null || !states.Any())
                {
                    states = TaskState.GetManagerDefaultList().Select(x => x.TaskStateId).ToArray();
                }
            }
            else if (curUser.Is(AdGroup.TaskTrackerProg))
            {
                if (states == null || !states.Any())
                {
                    states = TaskState.GetProgDefaultList().Select(x => x.TaskStateId).ToArray();
                }
                author = curUser.Sid;
                spec = curUser.Sid;
            }
            else
            {
                if (states == null || !states.Any())
                {
                    states = TaskState.GetUserDefaultList().Select(x => x.TaskStateId).ToArray();
                }
                author = curUser.Sid;
            }

            TaskTrackerContext db = new TaskTrackerContext();

                var list = db.Tasks.Where(x => x.Enabled &&
                ((String.IsNullOrEmpty(spec) || (!String.IsNullOrEmpty(spec) && x.SpecialistSid.Equals(spec))) || (String.IsNullOrEmpty(author) || (!String.IsNullOrEmpty(author) && x.CreatorSid.Equals(author))))
                //&& (String.IsNullOrEmpty(author) || (!String.IsNullOrEmpty(author) && x.CreatorSid.Equals(author))) 
                && (!states.Any() || !x.TaskStateId.HasValue || (states.Any() && x.TaskStateId.HasValue && states.Contains(x.TaskStateId.Value)))
                && (!projects.Any() || (projects.Any() && projects.Contains(x.ProjectId)))
                ).ToListAsync();//.OrderBy(x => x.Rank).ThenBy(x=> x.DateStartPlan).ToListAsync();
                //var list = db.Tasks.OrderBy(x => x.DateStartPlan);
                return await list;
            
        }

        public static async Task<IEnumerable<TaskClaim>> GetActiveTaskListAsync(AdUser curUser, string spec = null,
            string author = null, int[] projects = null)
        {
            var doneStates = Models.TaskState.GetTaskActiveStateIds().ToArray();
            var list = await GetListAsync(curUser, spec, author, doneStates, projects);
            
            return list;
        }

        public static IEnumerable<TaskClaim> GetList(string spec = null, string author = null, int[] states = null)
        {
            TaskTrackerContext db = new TaskTrackerContext();

                var list = db.Tasks.Where(x => x.Enabled &&
                (String.IsNullOrEmpty(spec) || (!String.IsNullOrEmpty(spec) && x.SpecialistSid.Equals(spec))) &&
                (String.IsNullOrEmpty(author) || (!String.IsNullOrEmpty(author) && x.CreatorSid.Equals(author)))
                && (!states.Any() || !x.TaskStateId.HasValue || (states.Any() && x.TaskStateId.HasValue && states.Contains(x.TaskStateId.Value)))
                ).OrderBy(x => x.DateStartPlan).ToList();
                //var list = db.Tasks.OrderBy(x => x.DateStartPlan);
                return list;
        }

        public async Task<int> Add(string creatorSid)
        {
            if (ProjectId <= 0) throw new ArgumentException("Не указан проект!");
            TaskTrackerContext db = new TaskTrackerContext();
            Enabled = true;
            CreatorSid = creatorSid;
            DateCreate = DateTime.Now;
            var state = await TaskState.GetFirstState();
            TaskStateId = state.TaskStateId;
            ModifierSid = CreatorSid;
            db.Tasks.Add(this);
            await db.SaveChangesAsync();
            await SetState(TaskId, state.TaskStateId, creatorSid);
            //var state = new Task2TaskState(TaskId, firstStateId, creatorSid);
            //db.TaskStateHistory.Add(state);
            //db.SaveChanges();
            //return true;
            await SendAddedNoticeToAuthor(TaskId);
            await SendAddedNoticeToManager(TaskId);

            return TaskId;
        }

        private static async Task SendAddedNoticeToAuthor(int taskId)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            string body =
                $"Ваша задача \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} успешно добавлена.<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            MessageHelper.SendNotice($"Задача успешно добавлена", body, true, null, to);
        }

        private static async Task SendAddedNoticeToManager(int taskId)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            string body =
                $"Добавлена новая задача \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name}.<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.Project.ManagerSid).Email);
            MessageHelper.SendNotice($"Новая задача", body, true, null, to);
        }

        private static async Task SendSetNoticeToSpecialist(int taskId)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            if (!String.IsNullOrEmpty(taskClaim.SpecialistSid))
            {
                string hostname = ConfigurationManager.AppSettings["hostname"];
                string catColor = String.Empty;
                if (taskClaim.TaskCategory != null)
                {
                    if (taskClaim.TaskCategory.SysName.ToUpper().Equals("BUG"))
                    {
                        catColor = "red";
                    }
                    if (taskClaim.TaskCategory.SysName.ToUpper().Equals("FEATURE"))
                    {
                        catColor = "green";
                    }
                }
                else
                {
                    //throw new ArgumentException("Не указана Категория задачи!");
                }
                string impColor = String.Empty;
                if (taskClaim.TaskImportant != null)
                {
                    if (taskClaim.TaskImportant.SysName.ToUpper().Equals("IMP"))
                    {
                        impColor = "red";
                    }
                    if (taskClaim.TaskImportant.SysName.ToUpper().Equals("NOTIMP"))
                    {
                        impColor = "green";
                    }
                }
                else
                {
                    //throw new ArgumentException("Не указана Важность задачи!");
                }
                string quickColor = String.Empty;
                if (taskClaim.TaskQuickly != null)
                {
                    if (taskClaim.TaskQuickly.SysName.ToUpper().Equals("QUICK"))
                    {
                        quickColor = "red";
                    }
                    if (taskClaim.TaskQuickly.SysName.ToUpper().Equals("NOTQUICK"))
                    {
                        quickColor = "green";
                    }
                }
                else
                {
                    //throw new ArgumentException("Не указана Срочность задачи!");
                }
                string body =
                    $"<span style='color: {catColor}'>{(taskClaim.TaskCategory != null ? taskClaim.TaskCategory.Name : null)}</span><br /><span style='color: {impColor}'>{(taskClaim.TaskImportant != null ?taskClaim.TaskImportant.Name : null)}</span><br /><span style='color: {quickColor}'>{(taskClaim.TaskQuickly != null ? taskClaim.TaskQuickly.Name : null)}</span><br />Новая <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>Задача №{taskClaim.TaskId}</a> от {AdHelper.GetUserBySid(taskClaim.CreatorSid).DisplayName} <br />{taskClaim.Descr}";
                MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.SpecialistSid).Email);
                string subj = $"[Новая][{(taskClaim.TaskCategory != null ? taskClaim.TaskCategory.Name : null)}][{(taskClaim.TaskImportant != null ? taskClaim.TaskImportant.Name : null)}][{(taskClaim.TaskQuickly != null ? taskClaim.TaskQuickly.Name:null)}]";
                MessageHelper.SendNotice(subj, body, true, null, to);
            }
            else
            {
                throw new ArgumentException("Не указан специалист!");
            }
        }

        private async static Task SetState(int taskId, int stateid, string creatorSid, string descr = null)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var state = new Task2TaskState(taskId, stateid, creatorSid, descr);
            db.TaskStateHistory.Add(state);
            db.SaveChanges();
            var task = db.Tasks.Single(x => x.TaskId == taskId);
            task.TaskStateId = state.TaskStateId;
            task.ModifierSid = creatorSid;
            await db.SaveChangesAsync();
        }

        public async static Task SetSetedState(int taskId, string creatorSid)
        {
            var state = await TaskState.GetSetedState();
            int stateId = state.TaskStateId;
            await SetState(taskId, stateId, creatorSid);
        }

        public async static Task SetTestState(int taskId, string creatorSid)
        {
            var state = await TaskState.GetTestState();
            int stateId = state.TaskStateId;
            await SetState(taskId, stateId, creatorSid);
        }

        public async static Task SetDoneState(int taskId, string creatorSid)
        {
            var state = await TaskState.GetDoneState();
            int stateId = state.TaskStateId;
            await SetState(taskId, stateId, creatorSid);
        }

        public async static Task SetPauseState(int taskId, string descr, string creatorSid)
        {
            var state = await TaskState.GetPauseState();
            int stateId = state.TaskStateId;
            await SetState(taskId, stateId, creatorSid, descr);
        }

        public async static Task SetWorkState(int taskId, string creatorSid)
        {
            var state = await TaskState.GetWorkState();
            int stateId = state.TaskStateId;
            await SetState(taskId, stateId, creatorSid);
        }
        private static async Task SendWorkNoticeToProg(int taskId, string creatorName)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            //creatorName = String.IsNullOrEmpty(creatorName) ? String.Empty : $" ({creatorName})";
            string body =
                $"{creatorName} отправил задачу \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} в работу.<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            MessageHelper.SendNotice($"Задача отправлена в работу", body, true, null, to);
        }
        private static async Task SendWorkNoticeToAuthor(int taskId,string creatorName)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            //creatorName = String.IsNullOrEmpty(creatorName) ? String.Empty : $" ({creatorName})";
            string body =
                $"{creatorName} отправил задачу \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} в работу.<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            MessageHelper.SendNotice($"Задача отправлена в работу", body, true, null, to);
        }
        public async static Task SetReworkState(int taskId, string descr, string creatorSid)
        {
            var state = await TaskState.GetReworkState();
            int stateId = state.TaskStateId;
            await SetState(taskId, stateId, creatorSid, descr);
        }
        private static async Task SendReworkNoticeToProg(int taskId, string descr, string creatorName)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            creatorName = String.IsNullOrEmpty(creatorName) ? String.Empty : $" ({creatorName})";
            string body =
                $"Задача \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} отправлена на доработку.<br />Комментарий{creatorName}:<br />{descr}<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            MessageHelper.SendNotice($"Задача отправлена на доработку", body, true, null, to);
        }
        private static async Task SendReworkNoticeToAuthor(int taskId, string descr, string creatorName)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            creatorName = String.IsNullOrEmpty(creatorName) ? String.Empty : $" ({creatorName})";
            string body =
                $"Задача \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} отправлена на доработку.<br />Комментарий{creatorName}:<br />{descr}<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            MessageHelper.SendNotice($"Задача отправлена на доработку", body, true, null, to);
        }
        private static async Task SendPauseNoticeToAuthor(int taskId, string descr)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            string body =
                $"Задача \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} приостановлена.<br />Комментарий:<br />{descr}<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            MessageHelper.SendNotice($"Задача приостановлена", body, true, null, to);
        }

        private static async Task SendDoneNoticeToAuthor(int taskId)
        {
            TaskClaim taskClaim = await TaskClaim.GetAsync(taskId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            string body =
                $"Ваша задача \"{taskClaim.Name}\" по проекту {taskClaim.Project.Name} выполнена.<br />Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a>";
            MailAddress to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email);
            string subj = $"Выполнена задача №{taskClaim.TaskId} {taskClaim.Project.Name}";
            MessageHelper.SendNotice(subj, body, true, null, to);
        }

        public async static Task GoNext(int taskId, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var task = await db.Tasks.SingleAsync(x => x.TaskId == taskId);
            var surState = task.TaskState;
        }

        

        public async static Task<ListResult<Task2TaskState>> GetStateHistory(int id, bool full = false)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var q = from x in db.TaskStateHistory
                    where x.TaskId == id
                    select x;

            var result = new ListResult<Task2TaskState>();
            result.Total = q.Count();
            int rows = full ? 100000 : 2;
            result.List = await q.Take(rows).OrderByDescending(x => x.DateCreate).ToListAsync();
            return result;

            //var list = await db.TaskStateHistory.Where(x => x.TaskId == id).OrderByDescending(x => x.DateCreate).ToListAsync();
            //return list;
            //var list = db.TaskStateHistory.Where(x => x.TaskId == TaskId).OrderByDescending(x => x.DateCreate).ToList();
            //return await list;
        }

        public async static Task SetCategory(int taskId, int categoryId, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            db.Tasks.Single(x => x.TaskId == taskId).TaskCategoryId = categoryId;
            await db.SaveChangesAsync();
        }

        public async static Task SetDone(int taskId,string creatorSid)
        {
            await SetDoneState(taskId, creatorSid);
            await SendDoneNoticeToAuthor(taskId);
        }

        public async static Task SetWork(int taskId, string creatorSid)
        {
            await SetWorkState(taskId, creatorSid);
            string creatorName = AdHelper.GetUserBySid(creatorSid).DisplayName;
            await SendWorkNoticeToAuthor(taskId, creatorName);
            await SendWorkNoticeToProg(taskId, creatorName);
        }

        public async static Task SetPause(int taskId, string descr, string creatorSid)
        {
            await SetPauseState(taskId, descr, creatorSid);
            await SendPauseNoticeToAuthor(taskId, descr);
        }

        public async static Task SetRework(int taskId, string descr, string creatorSid)
        {
            await SetReworkState(taskId, descr, creatorSid);
            string creatorName = AdHelper.GetUserBySid(creatorSid).DisplayName;
            await SendReworkNoticeToAuthor(taskId, descr, creatorName);
            await SendReworkNoticeToProg(taskId, descr, creatorName);
        }

        public async static Task SetSpecialist(int taskId, string specialistSid, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var task = db.Tasks.Single(x => x.TaskId == taskId);

            //if (task.TaskCategory == null) throw new ArgumentException("Не указана категория.");
            //if (task.TaskImportant == null) throw new ArgumentException("Не указана важность.");
            //if (task.TaskQuickly == null) throw new ArgumentException("Не указана срочность.");

            task.SpecialistSid = specialistSid;
            await db.SaveChangesAsync();
            await SetSetedState(taskId, creatorSid);
            await SendSetNoticeToSpecialist(taskId);
        }
        public async static Task SetTaskImportant(int taskId, int importantId, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            db.Tasks.Single(x => x.TaskId == taskId).TaskImportantId = importantId;
            await db.SaveChangesAsync();
        }
        public async static Task SetTaskQuickly(int taskId, int quicklyId, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            db.Tasks.Single(x => x.TaskId == taskId).TaskQuicklyId = quicklyId;
            await db.SaveChangesAsync();
        }

        public async Task<Task2TaskState> GetLastModify()
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskStateHistory.Where(x => x.TaskId == TaskId).OrderByDescending(x => x.DateCreate).FirstAsync();
        }

        public static async Task SaveRankAsync(string creatorSid, int taskId, int rank)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var task = db.Tasks.Single(x => x.TaskId == taskId);
            task.Rank = rank;
            task.RankChangeDate = DateTime.Now;
            task.RankChangerSid = creatorSid;
            await db.SaveChangesAsync();
        }

        public static async Task<ListResult<TaskAction>> GetActionListAsync(int id,bool full = false)
        {
            var task = new TaskClaim() {TaskId = id};
            return await task.GetActionListAsync(full);
        }

        public async Task<ListResult<TaskAction>>  GetActionListAsync(bool full = false)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var q = from x in db.TaskActions
                where x.TaskId == TaskId && x.Enabled
                select x;

            var result = new ListResult<TaskAction>();
            result.Total = q.Count();
            if (full)
            {
                result.List = await q.OrderByDescending(x => x.Id).ToListAsync();
            }
            else
            {
                result.List = await q.Take(2).OrderByDescending(x => x.Id).ToListAsync();
            }
            
            return result;


            //return await db.TaskActions.Where(x => x.TaskId == TaskId && Enabled).Take(rows).OrderByDescending(x=>x.Id).ToListAsync();
        }

        public static async Task<ListResult<Task2TaskState>> GetStateHistoryAsync(int id, bool full = false)
        {
            return await GetStateHistory(id, full);
        }

        public async Task<ListResult<Task2TaskState>> GetStateHistoryAsync(bool full = false)
        {
            return await GetStateHistory(TaskId, full);
        }
        public static async Task SetTaskNeedWorkList(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var task = await db.Tasks.SingleAsync(x => x.TaskId == id);
            task.NeedWorkList = true;
            await db.SaveChangesAsync();
        }
        public static async Task SetTaskNeedCheckpoints(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var task = await db.Tasks.SingleAsync(x => x.TaskId == id);
            task.NeedCheckpoints = true;
            await db.SaveChangesAsync();
        }

        ///// <summary>
        ///// Задача завершена или нет (выполнено либо отклонено
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static bool IsActive(int id)
        //{
        //    TaskTrackerContext db = new TaskTrackerContext();
        //    db.Tasks.Single(x=>x.TaskId==id).TaskState.
        //}
    }
}