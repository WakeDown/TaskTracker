using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
            if (form == null)
            {
                mail.From = new MailAddress("tt@un1t.group");
            }
            else
            {
                mail.From = form; 
            }

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.Host = "ums-1";
            client.Send(mail);
        }
    }
}