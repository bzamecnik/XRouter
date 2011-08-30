using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Requestor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Console.Error.WriteLine(@"Usage: Requestor.exe URI REQUEST_FILE RESPONSE_FILE");
                    Console.ReadLine();
                    return;
                }

                string uri = args[0];
                string requestFilename = args[1];
                string responseFilename = args[2];

                XDocument xRequest = XDocument.Load(requestFilename);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                request.Method = "Post";
                request.ContentType = "text/xml; charset=utf-8";

                // encoding
                Encoding encoding = Encoding.UTF8;

                string strRequest = xRequest.ToString();

                // send the message to the server
                byte[] bufferRequest = encoding.GetBytes(strRequest);
                request.ContentLength = bufferRequest.Length;

                Stream streamRequest = request.GetRequestStream();
                streamRequest.Write(bufferRequest, 0, bufferRequest.Length);
                streamRequest.Close();

                // wait for the response - synchronously
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                XDocument xResponse = XDocument.Load(response.GetResponseStream(), LoadOptions.SetLineInfo);

                xResponse.Save(responseFilename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
