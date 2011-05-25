using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XRouter.Gateway;

namespace XRouter.Adapters
{

    //! Sloucit Sync / Async adaptery dohromady
    // Nacteni v konfigurac jakyho je typu, ...


    class SOAPAsyncAdapter : IAdapter
    {
        public string Name { get; private set; }
        private XElement Configuration { get; set; }
        private IAdapterService Service { get; set; }

        #region AdapterConfiguration
        Dictionary<string, Uri> inputAddresses = new Dictionary<string, Uri>();
        Dictionary<string, Uri> outputAddress = new Dictionary<string, Uri>();
        List<HttpListener> listeners = new List<HttpListener>();
        int timeOut;
        #endregion

        //public void SOAPAsyncAdapter(XElement configuration)
        //{
        //    Configuration = configuration;
        //    Name = Configuration.Attribute(XName.Get("name")).Value;

        //    foreach (XElement outputElement in configuration.Elements("outputEndpoint")) {
        //        outputAddress.Add(outputElement.Attribute(XName.Get("name")).Value, new Uri(outputElement.Attribute(XName.Get("address")).Value));
        //    }
        //    foreach (XElement outputElement in configuration.Elements("output")) {
        //        inputAddresses.Add(outputElement.Attribute(XName.Get("name")).Value, new Uri(outputElement.Attribute(XName.Get("path")).Value));
        //    }
        //}

        public XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            try {
                if (outputAddress.ContainsKey(endpointName)) {
                    throw new Exception("Nenalezen odpovidajici OutputEndpoint ...");
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(outputAddress[endpointName]);

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
            } catch (Exception) {
                // logovani
            }
            return null;
        }

        public void Start(IAdapterService service)
        {
            this.Service = service;

            foreach (var pair in inputAddresses) {
                HttpListener listener = new HttpListener();

                listener.Prefixes.Add(pair.Value.AbsoluteUri);
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
            Service.ReceiveMessage(xDoc, listenerInfo.EndpointName);
        }


        public void Stop()
        {
            foreach (var listener in listeners) {
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
