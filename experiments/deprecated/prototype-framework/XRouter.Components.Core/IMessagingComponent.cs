using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Components.Core
{
    public interface IMessagingComponent
    {
        string Name { get; }

        void Run();
    }
}
