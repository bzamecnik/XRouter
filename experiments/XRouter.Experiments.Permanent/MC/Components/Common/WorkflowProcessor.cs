using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XRouter.Components.Core;
using XRouter.ComponentWatching;
using System.Windows;
using System.Threading.Tasks;

namespace XRouter.Experiments.Permanent.MC.Components.Common
{
    public abstract class WorkflowProcessor : IWatchableComponent
    {
        private List<InputPoint> InternalInputPoints { get; set; }
        private List<OutputPoint> InternalOutputPoints { get; set; }

        public ReadOnlyCollection<IMessageConsument> ConnectableInputPoints { get; private set; }
        public ReadOnlyCollection<IMessageProducent> ConnectableOutputPoints { get; private set; }

        protected ReadOnlyCollection<InputPoint> InputPoints { get; private set; }
        protected ReadOnlyCollection<OutputPoint> OutputPoints { get; private set; }        

        public string Name { get; private set; }

        public WorkflowProcessor(string name, int inputPoints, int outputPoints)
        {
            Name = name;

            InternalInputPoints = new List<InputPoint>();
            InternalOutputPoints = new List<OutputPoint>();
            InputPoints = new ReadOnlyCollection<InputPoint>(InternalInputPoints);
            OutputPoints = new ReadOnlyCollection<OutputPoint>(InternalOutputPoints);            

            for (int indexIterator = 0; indexIterator < inputPoints; indexIterator++) {
                int index = indexIterator;
                var inputPoint = new InputPoint(this, index);
                inputPoint.MessageReceived += delegate(Message message) {
                    Task.Factory.StartNew(delegate {
                        bool isHandled = HandleMessage(message, inputPoint);
                        if (isHandled) {
                            lock (inputPoint.MessagesLock){
                                inputPoint.Messages.Remove(message);
                            }                            
                        }
                    });
                };
                InternalInputPoints.Add(inputPoint);
            }

            for (int indexIterator = 0; indexIterator < outputPoints; indexIterator++) {
                int index = indexIterator;
                var outputPoint = new OutputPoint(this, index);
                InternalOutputPoints.Add(outputPoint);
            }

            ConnectableInputPoints = new ReadOnlyCollection<IMessageConsument>(InternalInputPoints.Cast<InputPoint>().ToArray());
            ConnectableOutputPoints = new ReadOnlyCollection<IMessageProducent>(InternalOutputPoints.Cast<IMessageProducent>().ToArray());
        }

        protected abstract bool HandleMessage(Message message, InputPoint source);

        protected class InputPoint : IMessageConsument
        {
            private WorkflowProcessor Owner { get; set; }
            public int Index { get; private set; }

            public object MessagesLock { get; private set; }
            public List<Message> Messages { get; private set; }

            public event Action<Message> MessageReceived = delegate { };

            public string Name {
                get { return string.Format("{0}_Input{1}", Owner.Name, Index); }
            }

            public InputPoint(WorkflowProcessor owner, int index)
            {
                Owner = owner;
                Index = index;

                MessagesLock = new object();
                Messages = new List<Message>();
            }

            void IMessageConsument.Send(Message message)
            {
                lock (MessagesLock) {
                    Messages.Add(message);
                }
                MessageReceived(message);
            }

            void IMessagingComponent.Run()
            {
            }
        }

        protected class OutputPoint : IMessageProducent
        {
            private WorkflowProcessor Owner { get; set; }
            public int Index { get; private set; }

            private event DispatchMessageDelegate InternalMessageSent = delegate { };
            event DispatchMessageDelegate IMessageProducent.MessageSent {
                add { InternalMessageSent += value; }	
                remove { InternalMessageSent -= value; }
            }

            public string Name {
                get { return string.Format("{0}_Output{1}", Owner.Name, Index); }
            }

            public OutputPoint(WorkflowProcessor owner, int index)
            {
                Owner = owner;
                Index = index;
            }

            public void Send(Message message)
            {
                InternalMessageSent(message);
            }

            void IMessagingComponent.Run()
            {
            }        
        }

        protected virtual FrameworkElement CreateRepresentationCore()
        {
            return null;
        }

        #region Implementation of WorkflowProcessor
        string IWatchableComponent.ComponentName { get { return Name; } }

        FrameworkElement IWatchableComponent.CreateRepresentation()
        {
            var result = CreateRepresentationCore();
            return result;
        }
        #endregion
    }
}
