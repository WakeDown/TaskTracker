using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace TaskTracker.Helpers
{
    public class MessageHelper
    {
        public static void SendNotice(string subj, string body, bool isBodyHtml, MailAddress form, params MailAddress[] to)
        {
            MailMessage mail = new MailMessage();
            foreach (MailAddress ma in to)
            {
                mail.To.Add(ma);
            }

            mail.Subject = subj;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;
            //if (form == null)
            //{
                mail.From = new MailAddress("delivery@unitgroup.ru");
            //}
            //else
            //{
            //    mail.From = form; 
            //}

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.office365.com";
            client.Credentials = new NetworkCredential("delivery@unitgroup.ru", "pRgvD7TL");
            Task.Run(()=> client.Send(mail));
        }
    }
}