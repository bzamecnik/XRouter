using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Gateway;

namespace XRouter.Adapters
{
    /// <summary>
    /// E-mail client adapter provides asynchronous sending of outgoing
    /// messages inside e-mails via SMTP. It does NOT provide receiving
    /// messages (eg. via POP3, IMAP).
    /// </summary>
    /// <remarks>
    /// <para>
    /// It can be used eg. for e-mail notifications where a XML message is
    /// stored in the attachment of an e-mail. One adapter instance represents
    /// a single e-mail template which can be sent to multiple e-mail adresses.
    /// </para>
    /// </remarks>
    [AdapterPlugin("E-mail sender adapter", "It can send e-mails with a XML message attached.")]
    class EMailClientAdapter : Adapter
    {
        [ConfigurationItem("SMTP host", "Host-name or IP adress of the SMTP server.", "")]
        public string SmtpHost { set; get; }

        [ConfigurationItem("SMTP port", "Port where the SMTP server listens.", 0)]
        public int SmtpPort { set; get; }

        // zde to bude chtit pridat validacni kod (tedy regex)
        [ConfigurationItem("Sender address", "Source address of the e-mail message sender.", "@")]
        public string From { set; get; }

        [ConfigurationItem("Sender name", "Display name of the e-mail message sender.", "XRouter")]
        public string FromDisplayName { set; get; }

        [ConfigurationItem("Subject", "The subject line of the e-mail message.", "")]
        public string Subject { set; get; }

        [ConfigurationItem("Body", "The message body.", "")]
        public string Body { set; get; }

        // zde to bude chtit pridat validacni kod (regex na kazdy prvek kolekce)
        // pozn.: pozor na to, ze dnes lze pouzivat v domenach i ruzne Unicode znaky!!
        [ConfigurationItem("Recipients", "A list of e-mail addresses of recipients of this e-mail message.", new[] { "@" })]
        public List<string> To { set; get; }

        protected override void Run()
        {
            // NOTE: the adapter does not listen to receive any messages
        }

        // podle meho nazoru by bylo lepsi, kdyby byla metadata objekt napr. Dictionary stringu, a to proto, abychom
        // programatorovi trochu usnadnili praci a spis proto, aby to bylo vzdy stejne, tj. takhle tam muze vlozit
        // cokoliv
        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            // create and init smtp client
            SmtpClient smtp = new SmtpClient();
            smtp.Host = this.SmtpHost;
            if (this.SmtpPort != 0)
            {
                smtp.Port = this.SmtpPort;
            }

            // create and init email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(this.From, this.FromDisplayName);
            mailMessage.Subject = this.Subject;
            mailMessage.Body = this.Body;
            foreach (String to in this.To)
            {
                mailMessage.To.Add(to);
            }

            if (message != null)
            {
                Attachment attachment = CreateAttachment(message, metadata);
                mailMessage.Attachments.Add(attachment);
            }

            // send email message
            smtp.Send(mailMessage);

            return null;
        }

        private static Attachment CreateAttachment(XDocument message, XDocument metadata)
        {
            string attachmentName = null;
            if (metadata != null)
            {
                attachmentName = metadata.Root.Value;
            }
            else
            {
                throw new InvalidOperationException();
            }

            Encoding attachmentEncoding = Encoding.UTF8;
            if (message.Declaration != null)
            {
                string xmlEncoding = message.Declaration.Encoding;
                if (!string.IsNullOrEmpty(xmlEncoding))
                {
                    attachmentEncoding = Encoding.GetEncoding(xmlEncoding);
                }
            }

            Byte[] bytes = attachmentEncoding.GetBytes(message.ToString());
            MemoryStream memoryStream = new MemoryStream(bytes);
            Attachment attachment = new Attachment(memoryStream, attachmentName);

            return attachment;
        }
    }
}
