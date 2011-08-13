using System;
using System.Net.Mail;
using DaemonNT.Logging;

namespace XRouter.Manager
{
    /// <summary>
    /// Reprezentuje odesilatko SMTP e-mailu.
    /// </summary>
    internal sealed class EMailSender
    {
        /// <summary>
        /// Hostname nebo IP adresa SMTP serveru. 
        /// </summary>
        private string smtpHost = null;

        /// <summary>
        /// Port na kterem posloucha SMTP server.
        /// </summary>
        private int? smtpPort = null;

        /// <summary>
        /// Adresa odesilate e-mailu. 
        /// </summary>
        private MailAddress from = null;

        /// <summary>
        /// Adresa prijemce e-mailu. 
        /// </summary>
        private MailAddress to = null;

        /// <summary>
        /// Odkaz na Daemon trace logger. 
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
        /// Odesle e-mailovou notifikaci, ktera je nastavena dle konfiguraci. V parametrech
        /// metody je mozno nastavit subject a body. Metoda nevyhazuje vyjimky, tj. pokud 
        /// dojde k vyjimecne udalosti, pak je pouze zalogovana. 
        /// </summary>     
        public void Send(string subject, string body)
        {
            try
            {
                this.logger.LogInfo(string.Format("Sending e-mail: '{0}'", body));

                // create smtp client
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
