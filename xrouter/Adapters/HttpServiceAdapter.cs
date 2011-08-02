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
using System.Net;
using XRouter.Common;
using ObjectConfigurator;

namespace XRouter.Adapters
{    
    /// <summary>
    /// Adapter poskytuje jednoduchy HTTP protocol listener a realizuje chování serveru request/response. 
    /// HTTP content muze byt libovolny XML dokument (typicky SOAP, tj. adapter umoznuje realizovat simple 
    /// RPC-style web service). 
    /// </summary>
    [AdapterPlugin("HttpServiceAdapter", "dodat description")]
    class HttpServiceAdapter : Adapter
    {
        // nastavit jako povinny        
        [ConfigurationItem("URI", "A URI string is composed of a scheme (http), a host, an optional port, and an optional path. An example of a complete prefix string is 'http://www.contoso.com:8080/customerData/'.", "http://localhost:8080/")]
        public string Uri { set; get; }
                
        private HttpListener listener = null;
       
        protected override void Run()
        {           
            this.listener = new HttpListener();
            this.listener.Prefixes.Add(this.Uri);
            this.listener.Start();

            while (true)
            {                
                HttpListenerContext context = null;
                try
                {
                    context = listener.GetContext();
                }
                catch (HttpListenerException)
                {                   
                    if (this.IsRunning)
                    {
                        throw;
                    }
                    else
                    {
                        break;
                    }
                }

                HttpListenerRequest request = context.Request;                                
                XDocument requestMessage = XDocument.Load(request.InputStream, LoadOptions.SetLineInfo);

                this.ReceiveMessageXml(requestMessage, null, null, context, delegate(Guid tokenGuid, XDocument resultMessage, XDocument sourceMetadata, object responseContext)
                {                   
                    HttpListenerResponse response = ((HttpListenerContext)responseContext).Response;
                                      
                    response.ContentType = request.ContentType;
                    string strResponse = resultMessage.ToString();
                    byte[] bufferResponse = request.ContentEncoding.GetBytes(strResponse);
                    response.ContentLength64 = bufferResponse.Length;

                    // pokud je spojeni ztraceno, pak vyhodi vyjimku                    
                    System.IO.Stream streamResponse = response.OutputStream;
                    streamResponse.Write(bufferResponse, 0, bufferResponse.Length);
                    streamResponse.Close();
                });
            }
        }
                           
        public override void OnTerminate()
        {         
            this.listener.Close();
        }

        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            throw new NotImplementedException();            
        }
    }
}
