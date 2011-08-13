using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.ComponentWatching;
using System.Windows.Controls;
using System.Windows;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public class BasicMessageFlowManagement_v1 : IBasicMessageFlowManagement_v1
    {
        public string Name { get; private set; }

        private IBasicMessageFlowManagementStorage_v1 Storage { get; set; }

        public BasicMessageFlowManagement_v1(string name, IBasicMessageFlowManagementStorage_v1 storage)
        {
            Name = name;
            Storage = storage;
        }

        public void Log(IFlowContext_v1 context, string message)
        {
            Storage.Store(context, message);
        }

        #region Implementation for IWatchableComponent
        string IWatchableComponent.ComponentName { get { return Name; } }

        FrameworkElement IWatchableComponent.CreateRepresentation()
        {
           return null;
        }
        #endregion
    }
}
