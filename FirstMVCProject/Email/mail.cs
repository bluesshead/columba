using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace FirstMVCProject.Email
{
    public class mail
    {
        public bool SendMail(string toMail, string subject, string emailbody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                //toMail = senderEmail;

                var client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                MailMessage mailmessage = new MailMessage(senderEmail, toMail, subject, emailbody);
                mailmessage.IsBodyHtml = true;
                mailmessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailmessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}