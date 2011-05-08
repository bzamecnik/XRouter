using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using XRouter.Data;

namespace DataAccessExample
{
    class Program
    {
        static void Main(string[] args)
        {
            MsSqlDataAccess dataAccess = new MsSqlDataAccess();

            /* all messages retrieval test */
            Console.WriteLine("Stored messages:");
            List<XmlDocument> messages = dataAccess.GetAllMessages();
            foreach (XmlDocument xml in messages)
            {
                WriteXML(xml);
            }
            Console.WriteLine();

            /* message retrieval test */
            int messageId = 5;
            Console.WriteLine("Message with ID {0}:", messageId);
            XmlDocument message = dataAccess.GetMessage(messageId);
            WriteXML(message);
            Console.WriteLine();

            /* update interests and get interests test */
            string componentName = "testComponent";
            List<string> interests = new List<string>();
            interests.Add("//nazdar/auto");
            interests.Add("//ahojky");
            dataAccess.UpdateComponentConfigInterests(componentName, interests);
            Console.WriteLine("Stored interests of {0}:", componentName);
            IEnumerable<string> storedInterests = dataAccess.GetComponentConfigInterests(componentName);
            foreach (string interest in storedInterests)
            {
                Console.WriteLine(interest);
            }
            Console.WriteLine();

            /* Save config */
            XmlDocument config = new XmlDocument();
            config.LoadXml("<conf><a>test</a><b>nic'</b></conf>");
            dataAccess.SaveConfig(config);
            WriteXML(dataAccess.GetConfig());

            /* Save message and tokens */
            XmlDocument msg = new XmlDocument();
            msg.LoadXml("<msg><a>t''est</a>zprava<b>nic'</b></msg>");
            int msgID = dataAccess.SaveMessage(DateTime.Now, msg, "TestPoint");
            Console.WriteLine("New message ID: {0}", msgID);
            XmlDocument token = new XmlDocument();
            token.LoadXml("<token>nejaka data<msg><a>t''est</a>zprava<b>nic'</b></msg></token>");
            dataAccess.SaveToken(msgID, componentName, DateTime.Now, token, TokenState.Received);

            /* Save logs / errors */
            dataAccess.SaveLog(componentName, DateTime.Now, 2, "pokusny log");
            XmlDocument error = new XmlDocument();
            error.LoadXml("<error>nejaka chyba</error>");
            dataAccess.SaveErrorAndLog(componentName, DateTime.Now, error, "pokusny error");

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
