using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Gateway;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using XRouter.Gateway.Implementation;
using System.IO;
using XRouter.Common;

namespace XRouter.Adapters.TestingPlugins
{
	class SOAPAsyncAdapter : IAdapter
	{
		public string Name { get; private set; }
		private XElement Configuration { get; set; }
		private IAdapterService Service { get; set; }		

		#region AdapterConfiguration
        Dictionary<string, string> listenerAddresses = new Dictionary<string, string>();
        Dictionary<string, Uri> outputAddress = new Dictionary<string, Uri>();
        List<HttpListener> listeners = new List<HttpListener>();
		int timeOut;
		#endregion

		public SOAPAsyncAdapter(XElement configuration, IAdapterService service)
		{
			this.Service = service;
			Configuration = configuration;
			Name = Configuration.Attribute(XName.Get("name")).Value;

            //! Nacteni listenerAddresses z konfigurace
            //! Nacteni outputAddress z konfigurace
		}

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
		{
			try
			{
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(outputAddress[address.EndPointName]);

				// konfigurace requestu SOAP nad HTTP
				request.ContentType = "text/xml; charset=utf-8";
				request.Headers.Add("SOAPAction", "http://tempuri.org/SWP/XRouter/SOAPAsyncAdapter/");
				request.Method = "Post";

                string strRequest = message.ToString();

				// odeslani zpravy serveru
				byte[] bufferRequest = System.Text.Encoding.UTF8.GetBytes(strRequest);
				request.ContentLength = bufferRequest.Length;
				Stream streamRequest = request.GetRequestStream();
				streamRequest.Write(bufferRequest, 0, bufferRequest.Length);
				streamRequest.Close();
			}
			catch (Exception)
			{
                // logovani
			}
            return null;
		}	

		public void Start()
		{			
			foreach (var pair in listenerAddresses)
			{
                HttpListener listener = new HttpListener();

                listener.Prefixes.Add(pair.Value);
                listener.Start();
                IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), new ListenerInfo(listener, pair.Key));
                
			}	
		}


        public void ListenerCallback(IAsyncResult result)
        {
            ListenerInfo listenerInfo = (ListenerInfo)result.AsyncState;
            HttpListener listener = listenerInfo.Listner;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            // zpracuje prichozi SOAP 
            HttpListenerRequest request = context.Request;           

            XDocument xDoc = XDocument.Load(request.InputStream);
            Token token = new Token();
            token.AddMessage(xDoc, new Guid().ToString());
            token.SourceAddress = new EndpointAddress(Name, listenerInfo.EndpointName);
            Service.ReceiveMessageAsync(token);
        }


		public void Stop()
		{
            foreach (var listener in listeners)
            {
                listener.Stop();
            }
            listeners.Clear();
		}

        private class ListenerInfo
        {
            public HttpListener Listner { get; private set; }

            public string EndpointName { get; private set; }

            public ListenerInfo(HttpListener listner, string endpointName)
            {
                Listner = Listner;
                EndpointName = EndpointName;
            }
        }
	}
}
