using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Management;
using XRouter.MessageProcessor;
using System.Threading.Tasks;

namespace XRouter.Scheduler.Implementation
{
    public class Scheduler : IScheduler
    {
        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        private List<IMessageProcessor> MessageProcessors { get; set; }

        public Scheduler(IXRouterManager xrouterManager, string name)
        {
            XRouterManager = xrouterManager;
            Name = name;
            XRouterManager.ConnectComponent(Name, this);
        }

        public void Initialize()
        {
            XElement configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/scheduler[@name=\"{0}\"]", Name)).XElement;

            MessageProcessors = new List<IMessageProcessor>();
            var targetProcessors = configuration.Element("targetProcessors").Elements("targetProcessor");
            foreach (var targetProcessor in targetProcessors) {
                string targetProcessorName = targetProcessor.Attribute(XName.Get("name")).Value;
                var messageProcessor = XRouterManager.GetComponent<IMessageProcessor>(targetProcessorName);
                MessageProcessors.Add(messageProcessor);
            }
        }

        public void ScheduleMessage(Message message)
        {
            Task.Factory.StartNew(delegate {
                var targetProcessor = MessageProcessors.First();
                targetProcessor.Process(message);
            });
        }
    }
}
