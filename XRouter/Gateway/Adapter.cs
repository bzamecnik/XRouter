using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using XRouter.Common;
using ObjectConfigurator;

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
        private volatile bool isRunning;
        /// <summary>
        /// Indicates that the adapter is running and can do its job.
        /// </summary>
        protected bool IsRunning
        {
            get { return isRunning; }
            private set { isRunning = value; }
        }

        [ConfigurationItem("Persist input tokens", "If checked the message flow state of each token originated from this adapter will be persistently stored (eg. in a database) after performing each step in the message flow processing.", false)]
        public bool AreInputTokensPersistent { get; private set; }

        private Task runTask;

        /// <summary>
        /// Gateway to which this adapter belongs.
        /// </summary>
        internal Gateway Gateway { get; set; }

        /// <summary>
        /// Identifier of the adapter.
        /// </summary>
        internal string AdapterName { get; set; }

        public XDocument config;
        /// <summary>
        /// Adapter configuration
        /// </summary>
        public XDocument Config
        {
            get
            {
                return config;
            }
            set
            {
                config = value;
                ObjectConfigurator.Configurator.LoadConfiguration(this, config);
            }
        }

        public Adapter()
        {
            // TODO: IsRunning should be set to true in the Start() method.
            IsRunning = true;
        }

        #region Starting and stopping the adapter

        /// <summary>
        /// Starts the adapter asynchronously, in a new thread, via the Run()
        /// method.
        /// </summary>
        internal void Start()
        {
            runTask = Task.Factory.StartNew(RunWrapper, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Performs the adapter's job, usually in a loop of a long running task.
        /// This method is indented to be redefined in a derived class. No action
        /// is the default behavior.
        /// </summary>
        protected abstract void Run();

        private void RunWrapper()
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                TraceLog.Exception(ex);
                TraceLog.Info(string.Format(
                    "Adapter {0} at gateway {1} was shut down due to an error (see the trace log).",
                    AdapterName, Gateway.Name));
                IsRunning = false;
                try
                {
                    // NOTE: even exceptions from OnTerminate() must not be lost
                    OnTerminate();
                }
                catch (Exception terminateEx)
                {
                    TraceLog.Exception(terminateEx);
                }
            }
        }

        /// <summary>
        /// Stops the adapter and calls OnTerminate() behavior of which can be
        /// defined in a derived class.
        /// </summary>
        internal void Stop()
        {
            try
            {
                IsRunning = false;
                OnTerminate();
                runTask.Wait();
            }
            catch (Exception ex)
            {
                TraceLog.Exception(ex);
            }
        }

        /// <summary>
        /// Action to be performed just after the adapter has been stopped.
        /// This method is indented to be redefined in a derived class. No
        /// action is the default behavior.
        /// </summary>
        public virtual void OnTerminate()
        {
        }

        #endregion

        #region Message input/output

        /// <summary>
        /// Sends an output message to a given output endpoint with an
        /// optional reply.
        /// </summary>
        /// <remarks>
        /// This method is synchronous, so that it can wait for a reply if
        /// there is any.
        /// </remarks>
        /// <param name="endpointName">output endpoint name</param>
        /// <param name="message">output message</param>
        /// <param name="metadata">message metadata</param>
        /// <returns>reply message if any; or null</returns>
        public abstract XDocument SendMessage(
            string endpointName,
            XDocument message,
            XDocument metadata);

        /// <summary>
        /// Receives an input message containing some plain-text data.
        /// </summary>
        /// <param name="message">plain-text message content</param>
        /// <param name="endpointName">input endpoint name; can be null</param>
        /// <param name="metadata">metadata about the input message; can be null</param>
        /// <param name="context">arbitrary context which should be retained
        /// until a possible reply; can be null</param>
        /// <param name="resultHandler">action to be performed with the reply
        /// message when the token processing has been finished</param>
        protected void ReceiveMessageData(
            string message,
            string endpointName,
            XDocument metadata,
            object context,
            MessageResultHandler resultHandler)
        {
            XDocument xDoc = new XDocument();
            xDoc.Add(new XElement(XName.Get("data"))
            {
                Value = message
            });
            this.ReceiveMessageXml(xDoc, endpointName, metadata, context, resultHandler);
        }

        /// <summary>
        /// Receives an input message containing XML data in a string.
        /// </summary>
        /// <param name="message">XML message content in a string</param>
        /// <param name="endpointName">input endpoint name; can be null</param>
        /// <param name="metadata">metadata about the input message; can be null</param>
        /// <param name="context">arbitrary context which should be retained
        /// until a possible reply; can be null</param>
        /// <param name="resultHandler">action to be performed with the reply
        /// message when the token processing has been finished</param>
        protected void ReceiveMessageXml(
            string message,
            string endpointName,
            XDocument metadata,
            object context,
            MessageResultHandler resultHandler)
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

        /// <summary>
        /// Receives an input message containing XML data in a stream.
        /// </summary>
        /// <param name="message">XML message content in a stream</param>
        /// <param name="endpointName">input endpoint name; can be null</param>
        /// <param name="metadata">metadata about the input message; can be null</param>
        /// <param name="context">arbitrary context which should be retained
        /// until a possible reply; can be null</param>
        /// <param name="resultHandler">action to be performed with the reply
        /// message when the token processing has been finished</param>
        protected void ReceiveMessageXml(
            Stream messageStream,
            string endpointName,
            XDocument metadata,
            object context,
            MessageResultHandler resultHandler)
        {
            try
            {
                XDocument xDoc = XDocument.Load(messageStream, LoadOptions.SetLineInfo);
                this.ReceiveMessageXml(xDoc, endpointName, metadata, context, resultHandler);
            }
            catch (Exception e)
            {
                TraceLog.Exception(e);
            }
        }

        /// <summary>
        /// Receives an input message containing XML data in an XDocument
        /// with no reply handling.
        /// </summary>
        /// <param name="message">XML message content in an XDocument</param>
        /// <param name="endpointName">input endpoint name; can be null</param>
        /// <param name="metadata">metadata about the input message; can be null</param>
        protected void ReceiveMessageXml(
            XDocument message,
            string endpointName,
            XDocument metadata)
        {
            ReceiveMessageXml(message, endpointName, metadata, null, null);
        }

        /// <summary>
        /// Receives an input message containing XML data in an XDocument.
        /// </summary>
        /// <remarks>
        /// This method is asynchronous and can simulate synchronous
        /// communication. The reply is send back from within a given result
        /// handler and a context is preseved.
        /// </remarks>
        /// <param name="message">XML message content in an XDocument</param>
        /// <param name="endpointName">input endpoint name; can be null</param>
        /// <param name="metadata">metadata about the input message; can be null</param>
        /// <param name="context">arbitrary context which should be retained
        /// until a possible reply; can be null</param>
        /// <param name="resultHandler">action to be performed with the reply
        /// message when the token processing has been finished</param>
        protected void ReceiveMessageXml(
            XDocument message,
            string endpointName,
            XDocument metadata,
            object context,
            MessageResultHandler resultHandler)
        {
            Token token = new Token();

            token.IsPersistent = AreInputTokensPersistent;
            token.SetSourceAddress(new EndpointAddress(Gateway.Name, AdapterName, endpointName));
            token.SaveSourceMetadata(metadata);
            token.AddMessage(Constants.InputMessageName, message);
            TraceLog.Info("Created token with GUID " + token.Guid);
            Gateway.ReceiveToken(token, context, resultHandler);
        }

        #endregion
    }
}
