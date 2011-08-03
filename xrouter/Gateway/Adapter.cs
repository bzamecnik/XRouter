using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using XRouter.Common;

namespace XRouter.Gateway
{
    /// <summary>
    /// This is a base class providing common code for concrete adapters.
    /// An adapter is a component of a gateway specialized in one type of
    /// communication with external systems (such as file exchange, web
    /// service, e-mail, etc.). It received incoming messages messages and
    /// passes them to the gateway for further processing. Also it sends
    /// outgoing processed messages away.
    /// </summary>
    /// <remarks>
    /// The incoming messages can be received either as XML with one of
    /// the ReceiveMessageXml() methods, or as plain-text data with the
    /// ReceiveMessageData() method. Outgoing messages can be send with the
    /// SendMessage() method.
    /// </remarks>
    public abstract class Adapter
    {
        protected bool IsRunning { get; private set; }

        private Task runTask;

        internal Gateway Gateway { get; set; }

        internal string AdapterName { get; set; }

        public XDocument config;
        public XDocument Config
        {
            get
            {
                return config;
            }
            set
            {
                config = value;
                System.Diagnostics.Debug.Assert(config != null);
                ObjectConfigurator.Configurator.LoadConfiguration(this, config);
            }
        }

        public Adapter()
        {
            IsRunning = true;
        }

        internal void Start()
        {
            runTask = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        internal void Stop()
        {
            IsRunning = false;
            OnTerminate();
            runTask.Wait();
        }

        protected void ReceiveMessageData(string message, string endpointName, XDocument metadata, object context, MessageResultHandler resultHandler)
        {
            XDocument xDoc = new XDocument();
            xDoc.Add(new XElement(XName.Get("data"))
            {
                Value = message
            });
            this.ReceiveMessageXml(xDoc, endpointName, metadata, context, resultHandler);
        }

        protected void ReceiveMessageXml(string message, string endpointName, XDocument metadata, object context, MessageResultHandler resultHandler)
        {
            try
            {
                XDocument xDoc = XDocument.Load(message, LoadOptions.SetLineInfo);
                this.ReceiveMessageXml(xDoc, endpointName, metadata, context, resultHandler);
            }
            catch (Exception e)
            {
                TraceLog.Exception(e);
            }            
        }

        protected void ReceiveMessageXml(Stream stream, string endpointName, XDocument metadata, object context, MessageResultHandler resultHandler)
        {
            try
            {
                XDocument xDoc = XDocument.Load(stream, LoadOptions.SetLineInfo);
                this.ReceiveMessageXml(xDoc, endpointName, metadata, context, resultHandler);
            }
            catch (Exception e)
            {
                TraceLog.Exception(e);
            }
        }

        protected void ReceiveMessageXml(XDocument message, string endpointName, XDocument metadata)
        {
            ReceiveMessageXml(message, endpointName, metadata, null, null);
        }

        protected void ReceiveMessageXml(XDocument message, string endpointName, XDocument metadata, object context, MessageResultHandler resultHandler)
        {
            Token token = new Token();

            token.SaveSourceAddress(new EndpointAddress(Gateway.Name, AdapterName, endpointName));
            token.SaveSourceMetadata(metadata);
            token.AddMessage(Constants.InputMessageName, message);
            TraceLog.Info("Created token with GUID " + token.Guid);
            Gateway.ReceiveToken(token, context, resultHandler);
        }

        public virtual void OnTerminate()
        {
        }

        protected abstract void Run();

        public abstract XDocument SendMessage(string endpointName, XDocument message, XDocument metadata);
    }
}
