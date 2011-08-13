using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Components.Core;
using XRouter.ComponentWatching;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XRouter.Experiments.Permanent.MC.Components.Common
{
    public class ManualMessageSender_v1 : IMessageProducent, IWatchableComponent
    {
        private static readonly string DefaultMessageContent = @"<Message>

</Message>
";

        public event DispatchMessageDelegate MessageSent = delegate { };

        public string Name { get; private set; }

        public ManualMessageSender_v1(string name)
        {
            Name = name;
        }

        public void Run() 
        {
        }        

        #region Implementation for IWatchableComponent        
        string IWatchableComponent.ComponentName { get { return Name; } }

        FrameworkElement IWatchableComponent.CreateRepresentation()
        {
            var uiContent = new TextBox {
                Text = DefaultMessageContent,
                AcceptsReturn = true,
                Width = 200,
                Height = 100,
                Margin = new Thickness(10),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var uiSend = new Button {
                Width = 80,
                Content = "Send",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 0, 10, 10)
            };

            uiSend.Click += delegate {
                string content = uiContent.Text;
                Task.Factory.StartNew(delegate {
                    XDocument xmlDocument = XDocument.Parse(content);
                    var message = new Message { RootElement = xmlDocument.Root };
                    MessageSent(message);
                });
            };

            var result = new Border {
                Child = new StackPanel {
                    Children = {
                        uiContent,
                        uiSend
                    }
                }
            };
            return result;
        }
        #endregion
    }
}
