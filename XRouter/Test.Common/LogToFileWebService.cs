using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace XRouter.Test.Common
{
    class LogToFileWebService
    {
        private HttpListener listener;

        public void Run()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8010/");
            listener.Start();
            int messagesProcessed = 0;

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                string fileName = string.Format("{0:0000000}.xml", messagesProcessed);
                using (Stream fileStream = File.Create(fileName))
                {
                    request.InputStream.CopyTo(fileStream);
                }    
                messagesProcessed++;
            }
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}
