using System;
using System.Net.Mail;
using DaemonNT.Logging;

namespace XRouter.Manager
{
    /// <summary>
    /// Provides a sender of e-mails to a pre-defined address via SMTP.
    /// </summary>
    /// <remarks>
    /// Support a plain unencrypted SMTP.
    /// </remarks>
    internal sealed class EMailSender
    {
        /// <summary>
        /// SMTP server hostname or IP address.
        /// </summary>
        private string smtpHost = null;

        /// <summary>
        /// SMTP server port. Can be null. 
        /// </summary>
        /// <remarks>If null value is specified a default port (usually 25)
        /// is used.</remarks>
        private int? smtpPort = null;

        /// <summary>
        /// E-mail address of the sender (a program).
        /// </summary>
        private MailAddress from = null;

        /// <summary>
        /// E-mail address of the recipient (typically an administrator).
        /// </summary>
        private MailAddress to = null;

        /// <summary>
        /// A reference to the trace logger.
        /// </summary>
        private TraceLogger logger = null;

        public EMailSender(string smtpHost, int? smtpPort, MailAddress from,
            MailAddress to, TraceLogger logger)
        {
            this.smtpHost = smtpHost;
            this.smtpPort = smtpPort;
            this.from = from;
            this.to = to;
            this.logger = logger;
        }

        /// <summary>
        /// Sends an e-mail notification to the pre-defined recipient.
        /// </summary>
        /// <remarks>
        /// Only subject and body can be given, the rest is pre-configured.
        /// In case of a problem, it is only logged, no exceptions are thrown
        /// outside this method.
        /// </remarks>
        public void Send(string subject, string body)
        {
            try
            {
                this.logger.LogInfo(string.Format("Sending e-mail:\nSubject: '{0}'\nBody:\n{1}", subject, body));

                // create an SMTP client
                SmtpClient smtp = new SmtpClient();
                smtp.Host = this.smtpHost;
                if (this.smtpPort != null)
                {
                    smtp.Port = this.smtpPort.Value;
                }

                // create e-mail message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = this.from;
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.To.Add(to);

                // send e-mail message
                smtp.Send(mailMessage);
            }
            catch (Exception e)
            {
                this.logger.LogException(e);
            }
        }
    }
}
