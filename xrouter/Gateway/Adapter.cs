using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;

namespace XRouter.Gateway
{
    public abstract class Adapter
    {
        protected bool IsRunning { get; private set; }

        private Task runTask;

        internal Gateway Gateway { get; set; }

        internal string AdapterName { get; set; }

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
            ReceiveMessageXml(message, endpointName, message, null, null);
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
