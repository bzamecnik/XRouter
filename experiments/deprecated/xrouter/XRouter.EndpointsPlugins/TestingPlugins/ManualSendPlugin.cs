using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using XRouter.Gateway;
using XRouter.Management;
using System.Threading.Tasks;

namespace XRouter.EndpointsPlugins.TestingPlugins
{
    class ManualSendPlugin : IEndpointsPlugin
    {
        public string Name { get; private set; }

        private List<IInputEndpoint> InternalInputEndpoints { get; set; }
        private List<IOutputEndpoint> InternalOutputEndpoints { get; set; }

        public IEnumerable<IInputEndpoint> InputEndpoints { get; private set; }
        public IEnumerable<IOutputEndpoint> OutputEndpoints { get; private set; }

        private IInputEndpoint Input { get; set; }

        private XElement Configuration { get; set; }

        private event ReceivedMessageHandler MessageReceived = delegate { };

        private Window window;

        public ManualSendPlugin(XElement configuration, IEndpointsPluginService service)
        {
            Configuration = configuration;
            Name = Configuration.Attribute(XName.Get("name")).Value;

            InternalInputEndpoints = new List<IInputEndpoint>();
            InternalOutputEndpoints = new List<IOutputEndpoint>();
            InputEndpoints = new ReadOnlyCollection<IInputEndpoint>(InternalInputEndpoints);
            OutputEndpoints = new ReadOnlyCollection<IOutputEndpoint>(InternalOutputEndpoints);

            Input = service.CreateInputEndpoint("Input", handler => MessageReceived += handler);
            InternalInputEndpoints.Add(Input);
        }

        public void Start()
        {
            var uiMessage = new TextBox {
                Margin = new Thickness(5),
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Text = @"<root>
</root>"
            };
            Grid.SetRow(uiMessage, 0);

            var uiSend = new Button {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 80,
                Content = "Send"
            };
            Grid.SetRow(uiSend, 1);
            uiSend.Click += delegate {
                string xmlText = uiMessage.Text;
                Task.Factory.StartNew(delegate {
                    var messageContent = XDocument.Parse(xmlText);
                    var message = new Message(messageContent, Input);
                    MessageReceived(message);
                });
            };

            window = new Window {
                Width = 350,
                Height = 300,
                Title = Name,
                Content = new Grid {
                    RowDefinitions = {
                        new RowDefinition { Height=new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height=new GridLength(1, GridUnitType.Auto) },
                    },
                    Margin = new Thickness(10),
                    Children = {
                        uiMessage,
                        uiSend
                    }
                }
            };
            window.Show();
        }

        public void Stop()
        {
            if (window != null) {
                window.Close();
                window = null;
            }
        }
    }
}
