using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Provider
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.Error.WriteLine(@"Usage: Provider.exe URI SERVICE_TYPE");
                    Console.ReadLine();
                    return;
                }

                string uri = args[0];                
                string serviceType = args[1];

                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(uri);
                listener.Start();
                Console.WriteLine(string.Format("{0} Listening...", serviceType));

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();

                    HttpListenerRequest request = context.Request;
                    XDocument requestMessage = XDocument.Load(request.InputStream, LoadOptions.SetLineInfo);

                    Console.WriteLine(requestMessage.ToString());
                    Console.WriteLine();

                    HttpListenerResponse response = context.Response;

                    response.ContentType = request.ContentType;

                    XDocument xResponse = XDocument.Parse("<S:Envelope xmlns:S='http://www.w3.org/2003/05/soap-envelope' xmlns:wsa='http://www.w3.org/2005/08/addressing'><S:Body><response status='OK'/></S:Body></S:Envelope>");
                    string strResponse = xResponse.ToString();
                    byte[] bufferResponse = request.ContentEncoding.GetBytes(strResponse);
                    response.ContentLength64 = bufferResponse.Length;

                    System.IO.Stream streamResponse = response.OutputStream;
                    streamResponse.Write(bufferResponse, 0, bufferResponse.Length);
                    streamResponse.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
