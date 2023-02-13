using ERCOFAS.Models;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for the email.
    /// </summary>
    public static class EmailHelpers
    {
        private static readonly DefaultDBContext _db = new DefaultDBContext();

        /// <summary>
        /// Sends email to target email address.
        /// </summary>
        /// <param name="emailAddress">The target email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="content">The content you want to send.</param>
        /// <param name="attachmentPath">The path of attachment file.</param>
        public static bool SendEmail(string emailAddress, string subject, string content, string attachmentPath = "")
        {
            var settings = _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");

            if (settings == null)
                return false;

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(settings.SMTPFromEmail)
                };
                mail.To.Add(new MailAddress(emailAddress));
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = content;

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    if (Directory.Exists(attachmentPath))
                    {
                        Attachment attachment = new Attachment(attachmentPath);
                        mail.Attachments.Add(attachment);
                    }
                }

                SmtpClient smtp = new SmtpClient
                {
                    Port = settings.SMTPPort,
                    Host = settings.SMTPServerName,
                    EnableSsl = settings.SMTPEnableSSL,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(settings.SMTPFromEmail, settings.SMTPPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientException)
            {
                return false;
            }
            return true;
        }
    }
}