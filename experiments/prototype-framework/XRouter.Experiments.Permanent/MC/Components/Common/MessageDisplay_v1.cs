using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Components.Core;
using XRouter.ComponentWatching;
using System.Windows;
using System.Windows.Controls;

namespace XRouter.Experiments.Permanent.MC.Components.Common
{
    public class MessageDisplay_v1 : IMessageConsument, IWatchableComponent
    {
        public string Name { get; private set; }

        public MessageDisplay_v1(string name)
        {
            Name = name;
        }

        public void Send(Message message)
        {
            uiOutput.Dispatcher.Invoke(new Action(delegate {
                if (uiAutoClear.IsChecked == true) {
                    uiOutput.Clear();
                }
                uiOutput.AppendText(DateTime.Now.ToString("HH:mm:ss"));
                uiOutput.AppendText(Environment.NewLine);
                uiOutput.AppendText(message.RootElement.ToString());
                uiOutput.AppendText(Environment.NewLine);
                uiOutput.AppendText(Environment.NewLine);
                uiOutput.ScrollToEnd();
            }));
        }        

        public void Run()
        {
        }

        #region Implementation for IWatchableComponent
        string IWatchableComponent.ComponentName { get { return Name; } }

        private TextBox uiOutput;
        private CheckBox uiAutoClear;

        FrameworkElement IWatchableComponent.CreateRepresentation()
        {
            uiOutput = new TextBox {
                IsReadOnly = true,
                AcceptsReturn = true,
                Width = 200,
                Height = 150,
                Margin = new Thickness(10),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            uiAutoClear = new CheckBox {
                Content = "Auto clear",
                IsChecked = true
            };

            var uiClear = new Button {
                Width = 80,
                Content = "Clear",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            uiClear.Click += delegate {
                uiOutput.Clear();
            };

            var result = new Border {
                Child = new StackPanel {
                    Children = {
                        uiOutput,
                        new Grid {
                            Margin = new Thickness(10, 0, 10, 10),
                            Children = {
                                uiAutoClear,
                                uiClear
                            }
                        }
                    }
                }
            };
            return result;
        }
        #endregion
    }
}
