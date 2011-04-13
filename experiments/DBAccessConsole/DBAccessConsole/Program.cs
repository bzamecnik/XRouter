using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace DBAccessConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            XRouter.DBAccess DBAccess = new XRouter.DBAccess();

            /* all messages retrieval test */
            Console.WriteLine("Stored messages:");
            List<XmlDocument> messages = DBAccess.GetAllMessages();
            foreach (XmlDocument xml in messages)
            {
                WriteXML(xml);
            }
            Console.WriteLine();

            /* message retrieval test */
            int messageID = 5;
            Console.WriteLine("Message with ID {0}:", messageID);
            XmlDocument message = DBAccess.GetMessage(messageID);
            WriteXML(message);
            Console.WriteLine();

            /* update interests and get interests test */
            string componentName = "testComponent";
            List<string> interests = new List<string>();
            interests.Add("//nazdar/auto");
            interests.Add("//ahojky");
            DBAccess.UpdateComponentConfigInterests(componentName, interests);
            Console.WriteLine("Stored interests of {0}:", componentName);
            IEnumerable<string> storedInterests = DBAccess.GetComponentConfigInterests(componentName);
            foreach (string interest in storedInterests)
            {
                Console.WriteLine(interest);
            }
            Console.WriteLine();

            /* Safe config test */
            XmlDocument config = new XmlDocument();
            config.LoadXml("<conf><a>test</a><b>nic'</b></conf>");
            DBAccess.SaveConfig(config);
            WriteXML(DBAccess.GetConfig());

            /* Save message and tokens */
            XmlDocument msg = new XmlDocument();
            msg.LoadXml("<msg><a>t''est</a>zprava<b>nic'</b></msg>");
            int msgID = DBAccess.SaveMessage(DateTime.Now, msg, "TestPoint");
            Console.WriteLine("New message ID: {0}", msgID);
            XmlDocument token = new XmlDocument();
            token.LoadXml("<token>nejaka data<msg><a>t''est</a>zprava<b>nic'</b></msg></token>");
            DBAccess.SaveToken(msgID, componentName, DateTime.Now, token, XRouter.TokenState.Received);

            /* Save logs / errors */
            DBAccess.SaveLog(componentName, DateTime.Now, 2, "pokusny log");
            XmlDocument error = new XmlDocument();
            error.LoadXml("<error>nejaka chyba</error>");
            DBAccess.SaveErrorAndLog(componentName, DateTime.Now, error, "pokusny error");

            //press any key..
            Console.WriteLine();
            Console.Write("Press any key...");
            Console.ReadLine();
        }

        static void WriteXML(XmlDocument xml)
        {
            StringWriter stringWriter = new StringWriter();
            xml.WriteTo(new XmlTextWriter(stringWriter));
            Console.WriteLine(stringWriter.ToString());
        }
    }
}
