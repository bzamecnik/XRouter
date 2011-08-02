using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Gateway;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Net.Mail;
using XRouter.Common;
using ObjectConfigurator;

namespace XRouter.Adapters
{    
    /// <summary>    
    /// Adapter poskytuje asynchronni odesilani e-mailovych zprav, ktere je mozno vyuzit jako e-mailove notifikace. 
    /// Do emailove notifikace je mozno prilozit XML zpravu. 
    /// </summary>
    class EMailClientAdapter : Adapter
    {        
        [ConfigurationItemAttribute("SmtpHost", "The name or IP adress of the host used for SMTP transactions.", "")]
        public string SmtpHost { set; get; }
        
        [ConfigurationItemAttribute("SmtpPort", "The port used for SMTP transactions.", 0)]
        public int SmtpPort { set; get; }

        // zde to bude chtit pridat validacni kod (tedy regex)
        [ConfigurationItem("From", "The from adress of the e-mail message.", "@")]
        public string From { set; get; }

        [ConfigurationItem("FromDisplayName", "The display name of the e-mail message.", "XRouter")]
        public string FromDisplayName { set; get; }
             
        [ConfigurationItem("Subject", "The subject line of the e-mail message.", "")]
        public string Subject { set; get; }

        [ConfigurationItem("Body", "The message body.", "")]
        public string Body { set; get; }

        // zde to bude chtit pridat validacni kod (regex na kazdy prvek kolekce)
        [ConfigurationItem("To", "The address collection that contains the recipients of this e-mail message.", new [] {"@"})]
        public List<string> To { set; get; }

        protected override void Run()
        { }

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
