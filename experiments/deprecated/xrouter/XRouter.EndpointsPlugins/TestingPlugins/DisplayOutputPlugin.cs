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
    class DisplayOutputPlugin : IEndpointsPlugin
    {
        public string Name { get; private set; }

        private List<IInputEndpoint> InternalInputEndpoints { get; set; }
        private List<IOutputEndpoint> InternalOutputEndpoints { get; set; }

        public IEnumerable<IInputEndpoint> InputEndpoints { get; private set; }
        public IEnumerable<IOutputEndpoint> OutputEndpoints { get; private set; }

        private IOutputEndpoint Output { get; set; }

        private XElement Configuration { get; set; }

        private TextBox uiOutput;
        private Window window;

        public DisplayOutputPlugin(XElement configuration, IEndpointsPluginService service)
        {
            Configuration = configuration;
            Name = Configuration.Attribute(XName.Get("name")).Value;

            InternalInputEndpoints = new List<IInputEndpoint>();
            InternalOutputEndpoints = new List<IOutputEndpoint>();
            InputEndpoints = new ReadOnlyCollection<IInputEndpoint>(InternalInputEndpoints);
            OutputEndpoints = new ReadOnlyCollection<IOutputEndpoint>(InternalOutputEndpoints);

            Output = service.CreateOutputEndpoint("Output", SendAsync);
            InternalOutputEndpoints.Add(Output);
        }

        private void SendAsync(Message message, MessageSendResultHandler resultHandler)
        {
            window.Dispatcher.Invoke(new Action(delegate {                                
                uiOutput.Text += string.Format("From {0}:", message.Source.Address) + Environment.NewLine;
                uiOutput.Text += message.Content.ToString();
                uiOutput.Text += Environment.NewLine + Environment.NewLine;

                Task.Factory.StartNew(delegate {
                    resultHandler(true);
                });
            }));
        }

        public void Start()
        {
            uiOutput = new TextBox {
                Margin = new Thickness(5),
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            Grid.SetRow(uiOutput, 0);

            window = new Window {
                Width = 350,
                Height = 400,
                Title = Name,
                Content = new Grid {
                    RowDefinitions = {
                        new RowDefinition { Height=new GridLength(1, GridUnitType.Star) }
                    },
                    Margin = new Thickness(10),
                    Children = {
                        uiOutput
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
