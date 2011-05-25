using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Xml.Linq;

namespace XRouter.Gateway
{
    public interface IAdapter
    {
        void Start(IAdapterService service);
        void Stop();

        XDocument SendMessage(string endpointName, XDocument message, XDocument metadata);
    }
}
