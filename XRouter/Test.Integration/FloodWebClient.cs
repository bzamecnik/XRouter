using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using XRouter.Test.Common;

namespace XRouter.Test.Integration
{
    class FloodWebClient
    {
        public string ServiceUri { set; get; }

        public string ContentType = "text/xml; charset=utf-8";

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int TimeOut = 7;

        /// <summary>
        /// Sends a fixed number of messages and measure the elapsed time.
        /// </summary>
        /// <param name="messageCount"></param>
        public void SendFixedCountFlood(int messageCount, int messageSize)
        {
            Console.WriteLine("HTTP client - flood generator: sending {0} messages of {1} bytes each",
                messageCount, messageSize);
            TextGenerator gen = new TextGenerator()
            {
                MinMessageLength = messageSize,
                MaxMessageLength = messageSize
            };
            XDocument message = new XDocument();
            message.Add(new XElement("message", gen.GenerateMessage()));

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < messageCount; i++)
            {
                SendMessage(message);
            }
            sw.Stop();
            Console.WriteLine("Elapsed time: {0}, average: {1} messages/sec",
                sw.ElapsedMilliseconds, messageCount / (0.001 * sw.ElapsedMilliseconds));
        }

        XDocument SendMessage(XDocument message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.ServiceUri);
            request.Timeout = this.TimeOut * 1000;

            request.Method = "Post";
            request.ContentType = this.ContentType;

            // send the message to the server
            Encoding encoding = Encoding.UTF8;
            string strRequest = message.ToString();
            byte[] bufferRequest = encoding.GetBytes(strRequest);
            request.ContentLength = bufferRequest.Length;

            Stream streamRequest = request.GetRequestStream();
            streamRequest.Write(bufferRequest, 0, bufferRequest.Length);
            streamRequest.Close();

            //// wait for the response - synchronously
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //XDocument xResponse = XDocument.Load(response.GetResponseStream(), LoadOptions.SetLineInfo);

            //return xResponse;
            return null;
        }
    }
}
