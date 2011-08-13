using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XRouter.Data;

namespace DataAccessExample
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataAccess_Old dataAccess = new MsSqlDataAccess();

            /* all messages retrieval test */
            Console.WriteLine("Stored messages:");
            List<XDocument> messages = dataAccess.GetAllMessages();
            foreach (XDocument xml in messages)
            {
                WriteXML(xml);
            }
            Console.WriteLine();

            /* message retrieval test */
            int messageId = 5;
            Console.WriteLine("Message with ID {0}:", messageId);
            XDocument message = dataAccess.GetMessage(messageId);
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

            /* Safe config test */
            XDocument config = XDocument.Parse("<conf><a>test</a><b>nic'</b></conf>");
            dataAccess.SaveConfig(config);
            WriteXML(dataAccess.GetConfig());

            /* Save message and tokens */
            XDocument msg = XDocument.Parse("<msg><a>t''est</a>zprava<b>nic'</b></msg>");
            int msgID = dataAccess.SaveMessage(DateTime.Now, msg, "TestPoint");
            Console.WriteLine("New message ID: {0}", msgID);
            XDocument token = XDocument.Parse("<token>nejaka data<msg><a>t''est</a>zprava<b>nic'</b></msg></token>");
            dataAccess.SaveToken(msgID, componentName, DateTime.Now, token, TokenState.Received);

            /* Save logs / errors */
            dataAccess.SaveLog(componentName, DateTime.Now, 2, "pokusny log");
            XDocument error = XDocument.Parse("<error>nejaka chyba</error>");
            dataAccess.SaveErrorAndLog(componentName, DateTime.Now, error, "pokusny error");

            //press any key..
            Console.WriteLine();
            Console.Write("Press any key...");
            Console.ReadLine();
        }

        static void WriteXML(XDocument xml)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                xml.WriteTo(xw);
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
