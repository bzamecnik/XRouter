using System;
using System.Net;
using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Gateway;

namespace XRouter.Adapters
{
    /// <summary>
    /// HTTP service adapter provides a simple web service listener (server).
    /// In can receive XML messages in requests from remote clients and respond
    /// to them in RPC style. The XML content can be arbitrary (typically SOAP).
    /// </summary>
    [AdapterPlugin("HTTP service adapter", "Provides a simple web service listener which can receive messages.")]
    public class HttpServiceAdapter : Adapter
    {
        // nastavit jako povinny
        [ConfigurationItem("Listener URI prefix", "It is composed of a scheme (http), host name, (optional) port, and (optional) path. Example: 'http://www.example.com:8080/path/'.", "http://localhost:8080/")]
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
