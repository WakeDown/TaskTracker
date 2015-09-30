using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using TaskTracker.Helpers;
using TaskTracker.Objects;

namespace TaskTracker.Models
{
    public class TaskComment
    {
        [Key]
        public int TaskCommentId { get; set; }
        public string Text { get; set; }
        public int TaskClaimId { get; set; }
        public virtual TaskClaim TaskClaim { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatorSid { get; set; }
        public bool Enabled { get; set; }
        public DateTime? DateDelete { get; set; }
        public string DeleterSid { get; set; }

        public TaskComment()
        {
            
        }

        public static async Task<TaskComment> GetAsync(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return await db.TaskComments.SingleOrDefaultAsync(x => x.TaskCommentId == id);
        }

        public static TaskComment Get(int id)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            return db.TaskComments.SingleOrDefault(x => x.TaskCommentId == id);
        }
        
        public static async Task<IEnumerable<TaskComment>> GetListAsync(int taskId)
        {
            TaskTrackerContext db = new TaskTrackerContext();

            var list = db.TaskComments.Where(x => x.Enabled && x.TaskClaimId == taskId).OrderBy(x => x.DateCreate).ToListAsync();
            return await list;
        }

        public async Task<int> AddAsync(string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            Enabled = true;
            CreatorSid = creatorSid;
            DateCreate = DateTime.Now;
            db.TaskComments.Add(this);
            await db.SaveChangesAsync();
            await SendNoticeToAuthor();
            return TaskCommentId;

        }

        public static async Task CloseAsync(int id, string creatorSid)
        {
            TaskTrackerContext db = new TaskTrackerContext();
            var checkpoint = await db.TaskComments.SingleAsync(x => x.TaskCommentId == id);
            checkpoint.Enabled = false;
            checkpoint.DeleterSid = creatorSid;
            checkpoint.DateDelete = DateTime.Now;
            await db.SaveChangesAsync();
        }

        private async Task SendNoticeToAuthor()
        {
            TaskClaim taskClaim = await TaskClaim.Get(TaskClaimId);
            string hostname = ConfigurationManager.AppSettings["hostname"];
            string body =
                $"Новый комментарий по задаче \"{taskClaim.Name}\" в проекте {taskClaim.Project.Name}.<br />{AdHelper.GetUserBySid(CreatorSid).DisplayName} пишет:<br />{Text}<p>Ссылка - <a href='{hostname}/Task/Card/{taskClaim.TaskId}'>{hostname}/Task/Card/{taskClaim.TaskId}</a></p>";

            MailAddress to = null;
            if (taskClaim.CreatorSid.Equals(CreatorSid))
            {
                to = new MailAddress(AdHelper.GetUserBySid(taskClaim.SpecialistSid).Email);
            }
            else
            {
                to = new MailAddress(AdHelper.GetUserBySid(taskClaim.CreatorSid).Email); 
            }
            MessageHelper.SendNotice($"Новый комментарий", body, true, null, to);
        }
    }
}