using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Xml.Linq;
using System.Threading.Tasks;

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

        protected void ReceiveMessage(XDocument message, string endpointName, XDocument metadata = null, MessageResultHandler resultHandler = null)
        {
            Token token = new Token();
            token.SaveSourceAddress(new EndpointAddress(Gateway.Name, AdapterName, endpointName));
            token.SaveSourceMetadata(metadata);
            token.AddMessage(Constants.InputMessageName, message);
            Gateway.ReceiveToken(token, resultHandler);
        }

        public virtual void OnTerminate()
        {
        }

        protected abstract void Run();

        public abstract XDocument SendMessage(string endpointName, XDocument message, XDocument metadata);
    }
}
