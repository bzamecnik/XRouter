//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using XRouter.Gateway;
//using XRouter.Management;
//using System.Xml.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Net;
//using System.Xml;
//using XRouter.Gateway.Implementation;
//using System.IO;
//using XRouter.Common;

//namespace XRouter.Adapters.TestingPlugins
//{

//    //! Adapter bezi v jednom tasku, naslouchani je asynchroni, begincontext, kdyz prijde zprava vytvori se dalsi task pro jeji obsluhu

//    class SOAPSyncAdapter : IAdapter
//    {
//        public string Name { get; private set; }
//        private XElement Configuration { get; set; }
//        private IAdapterService Service { get; set; }

//        List<HttpListener> listeners = new List<HttpListener>();

//        #region AdapterConfiguration
//        Dictionary<string, string> listenerAddresses = new Dictionary<string, string>();
//        int timeOut;
//        #endregion

//        public SOAPSyncAdapter(XElement configuration, IAdapterService service)
//        {
//            this.Service = service;
//            Configuration = configuration;
//            Name = Configuration.Attribute(XName.Get("name")).Value;

//            //! Nacteni listenerAddresses z konfigurace
//        }

//        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
//        {
//            try
//            {
//                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("" /* Adresa kam se posle, bude v Tokenu ?? */);

//                // konfigurace requestu SOAP nad HTTP
//                request.ContentType = "text/xml; charset=utf-8";
//                request.Headers.Add("SOAPAction", "http://tempuri.org/SWP/XRouter/SOAPSyncAdapter/");
//                request.Method = "Post";

//                string strRequest = message.ToString();

//                // odeslani zpravy serveru        
//                byte[] bufferRequest = System.Text.Encoding.UTF8.GetBytes(strRequest);
//                request.ContentLength = bufferRequest.Length;
//                Stream streamRequest = request.GetRequestStream();
//                streamRequest.Write(bufferRequest, 0, bufferRequest.Length);
//                streamRequest.Close();
//            }
//            catch (Exception)
//            {
//                // logovani
//            }
//            return null; //! vraceni vysledku
//        }
		

//        private void adapterRun(string listenerAddress)
//        {
//            HttpListener listener = new HttpListener();


//            listener.Prefixes.Add(listenerAddress);
//            listener.Start();

//            while (true)
//            {
//                // ceka na zpravu
//                HttpListenerContext context = listener.GetContext();

//                // zpracuje prichozi SOAP 
//                HttpListenerRequest request = context.Request;
//                XDocument xDoc = XDocument.Load(request.InputStream);
		
//                Token token = ReceiveMessage(xDoc);

//                if (token != null)
//                {
//                    // Odeslani odpovedi zpet
//                    // vytvoreni a odeslani odpovedi                                 
//                    HttpListenerResponse response = context.Response;
//                    response.ContentType = "text/xml; charset=utf-8";

//                    string strResponse;
//                    strResponse = token.ResultMessage().ToString();

//                    byte[] bufferResponse = System.Text.Encoding.UTF8.GetBytes(strResponse);
//                    response.ContentLength64 = bufferResponse.Length;
//                    System.IO.Stream streamResponse = response.OutputStream;
//                    streamResponse.Write(bufferResponse, 0, bufferResponse.Length);
//                    streamResponse.Close();
//                }
//                else
//                {
//                    // vraci exception do soap timeout
//                }
//            }
//        }	

//        public void Start()
//        {
//            foreach (string listenerAddress in listenerAddresses)
//            {
//                HttpListener listener = new HttpListener();

//                listener.Prefixes.Add(listenerAddress);
//                listener.Start();
//                IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);

//            }
//        }

//        private void ListenerCallback(IAsyncResult result)
//        {
//            HttpListener listener = (HttpListener)result.AsyncState;
//            // Call EndGetContext to complete the asynchronous operation.
//            //! Znova otevrit spojeni ???
//            HttpListenerContext context = listener.EndGetContext(result);
//            // zpracuje prichozi SOAP 
//            HttpListenerRequest request = context.Request;
//            XDocument xDoc = XDocument.Load(request.InputStream);
//            //! Doplnit odpoved






//            Token token = Token.Create(xDoc, Input);
//            // pojede v tasku ?
//            Token responseToken = Service.ReciveMessage(token, Input);



//            // Obtain a response object.
//            HttpListenerResponse response = context.Response;
//            // Construct a response.
//            string responseString = responseToken.ResultMessage().ToString();
//            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
//            // Get a response stream and write the response to it.
//            response.ContentLength64 = buffer.Length;
//            System.IO.Stream output = response.OutputStream;
//            output.Write(buffer, 0, buffer.Length);
//            // You must close the output stream.
//            output.Close();



//        }

//        public void Stop()
//        {
//            foreach (var listener in listeners)
//            {
//                listener.Stop();
//            }
//            listeners.Clear();
//        }
//    }
//}
