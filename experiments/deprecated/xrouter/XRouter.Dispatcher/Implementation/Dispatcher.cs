using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XRouter.Management;
using XRouter.Processor;

namespace XRouter.Dispatcher.Implementation
{
    public class Dispatcher : IDispatcher
    {
        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        private List<IProcessor> Processors { get; set; }

        public Dispatcher(IXRouterManager xrouterManager, string name)
        {
            XRouterManager = xrouterManager;
            Name = name;
            XRouterManager.ConnectComponent<IDispatcher>(Name, this);
        }

        #region IXRouterComponent interface

        public void Initialize()
        {
            XElement configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/dispatcher[@name=\"{0}\"]", Name)).XElement;

            Processors = new List<IProcessor>();
            var targetProcessors = configuration.Element("targetProcessors").Elements("processor");
            foreach (var targetProcessor in targetProcessors) {
                string targetProcessorName = targetProcessor.Attribute(XName.Get("name")).Value;
                var processor = XRouterManager.GetComponent<IProcessor>(targetProcessorName);
                Processors.Add(processor);
            }
        }

        #endregion

        #region IDispatcher interface

        public void Dispatch(Message message)
        {
            var processor = ChooseProcessor();
            processor.Process(message);
            
            // TODO:
            // handle a situation when the pool is not available or does not
            // accept the message
        }

        #endregion

        private IProcessor ChooseProcessor()
        {
            // TODO: there could be some sofisticated load balancing
            return Processors.First();
        }
    }
}
