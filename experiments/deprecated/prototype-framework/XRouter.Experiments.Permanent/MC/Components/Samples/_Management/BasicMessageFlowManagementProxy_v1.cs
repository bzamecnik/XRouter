using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using System.Windows;
using System.Windows.Controls;
using XRouter.ComponentWatching;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public class BasicMessageFlowManagementProxy_v1 : IBasicMessageFlowManagement_v1
    {
        private IBasicMessageFlowManagement_v1 Target { get; set; }

        private int callsCount = 0;

        public BasicMessageFlowManagementProxy_v1(IBasicMessageFlowManagement_v1 target)
        {
            Target = target;
        }

        public void Log(IFlowContext_v1 context, string message)
        {
            Target.Log(context, message);
            callsCount++;

            uiCallsCount.Dispatcher.Invoke(new Action(delegate {
                uiCallsCount.Text = string.Format("Called {0} times", callsCount);
            }));
        }

        #region Implementation for IWatchableComponent
        private TextBlock uiCallsCount;

        string IWatchableComponent.ComponentName { get { return string.Format("Proxy_{0}", Target.ComponentName); } }

        FrameworkElement IWatchableComponent.CreateRepresentation()
        {
            uiCallsCount = new TextBlock {
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            return uiCallsCount;
        }
        #endregion
    }
}
