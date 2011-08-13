using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.ComponentWatching;
using XRouter.Components.Core;
using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;

namespace XRouter.Experiments.Permanent.MC.Components.Common
{
    public class Connector_v1 : IWatchableComponent
    {
        private Queue<Message> messages = new Queue<Message>();

        private FrameworkElement uiMessageProcessor;
        private TextBlock uiMessageView;
        private CheckBox uiAutoPass;

        [HideReference]
        private IMessageProducent _producent;
        public IMessageProducent Producent {
            get { return _producent; }
            private set { _producent = value; }
        }

        public IMessageConsument Consument { get; private set; }

        public string ComponentName {
            get {
                return string.Format("{0}->{1}", Producent.Name, Consument.Name);
            }
        }

        public Connector_v1(IMessageProducent producent, IMessageConsument consument)
        {
            Producent = producent;
            Consument = consument;

            producent.MessageSent += new DispatchMessageDelegate(producent_MessageSent);
        }

        void producent_MessageSent(Message message)
        {
            uiMessageProcessor.Dispatcher.Invoke(new Action(delegate {
                messages.Enqueue(message);
                UpdateProcessor();
            }));
        }

        private void UpdateProcessor()
        {
            if (uiAutoPass.IsChecked == true) {
                while (messages.Count > 0) {
                    Message message = messages.Dequeue();
                    Task.Factory.StartNew(delegate {
                        Consument.Send(message);
                    });
                }
            }
            if (messages.Count > 0) {
                Message peakMessage = messages.Peek();
                uiMessageView.Text = string.Format("Enqueued {0} messages", messages.Count);
                uiMessageView.ToolTip = peakMessage.RootElement.ToString();
                uiMessageProcessor.Visibility = Visibility.Visible;
            } else {
                uiMessageProcessor.Visibility = Visibility.Collapsed;
            }
        }

        FrameworkElement IWatchableComponent.CreateRepresentation()
        {
            uiMessageView = new TextBlock {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 2, 5, 0)
            };

            var uiPass = new Button {
                Content = "Pass",
                Width = 70,
                Margin = new Thickness(5)
            };
            uiPass.Click += delegate {
                if (messages.Count > 0) {
                    Message message = messages.Dequeue();
                    Task.Factory.StartNew(delegate {
                        Consument.Send(message);
                    });
                }
                UpdateProcessor();
            };

            var uiDismiss = new Button {
                Content = "Dismiss",
                Width = 70,
                Margin = new Thickness(5, 5, 0, 5)
            };
            uiDismiss.Click += delegate {
                if (messages.Count > 0) {
                    messages.Dequeue();
                }
                UpdateProcessor();
            };

            uiAutoPass = new CheckBox {
                Content = "Always pass",
                IsChecked = true,
                Margin = new Thickness(5, 0, 5, 0)
            };
            uiAutoPass.Checked += delegate {
                UpdateProcessor();
            };

            uiMessageProcessor = new StackPanel {
                Children = {
                    uiMessageView,
                    new StackPanel {
                        Orientation = Orientation.Horizontal,
                        Children = {
                            uiDismiss,
                            uiPass
                        }
                    }                    
                }
            };
            UpdateProcessor();

            var result = new Border {
                Child = new StackPanel {
                    Children = {
                        uiMessageProcessor,
                        uiAutoPass
                    }
                }
            };
            return result;
        }
    }
}
