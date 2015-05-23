using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Net;
using System.Net.Mail;
using LifeManagement.Models.DB;

namespace LifeManagement.Logic
{
    public static class EmailManager
    {
        private static string smtpServer = "smtp.gmail.com";
        private static string from = "lifemanagementlime@gmail.com";
        private static string password = "lifeManagement1";
        private static string mailto = "ann.nayasan@gmail.com";
        public static void SendMail(Feedback feedback, string Login)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = "Feedback from "+Login;
                mail.Body = feedback.Subject+"\n"+feedback.Message;
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(from, password);
                client.Host = smtpServer;
                client.Port = 587;
                client.EnableSsl = true;                
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}