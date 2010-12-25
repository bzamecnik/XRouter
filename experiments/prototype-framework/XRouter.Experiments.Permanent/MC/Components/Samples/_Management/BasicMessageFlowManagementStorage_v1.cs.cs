using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using XRouter.ComponentWatching;
using System.Windows.Controls;
using XRouter.Experiments.Permanent.MC.Components.Common;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public class BasicMessageFlowManagementStorage_v1 : IBasicMessageFlowManagementStorage_v1
    {
        public string Name { get; private set; }

        public BasicMessageFlowManagementStorage_v1(string name)
        {
            Name = name;
        }

        public void Store(IFlowContext_v1 context, string message)
        {
            uiOutput.Dispatcher.Invoke(new Action(delegate {
                if (uiAutoClear.IsChecked == true) {
                    uiOutput.Clear();
                }
                uiOutput.AppendText(DateTime.Now.ToString("HH:mm:ss"));
                uiOutput.AppendText(Environment.NewLine);
                uiOutput.AppendText(message);
                uiOutput.AppendText(Environment.NewLine);
                uiOutput.AppendText(Environment.NewLine);
                uiOutput.ScrollToEnd();
            }));
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
                IsChecked = false
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
                        new TextBlock {
                            Text = "Logs storage:",
                            Margin = new Thickness(10, 0, 10, 0)
                        },
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
