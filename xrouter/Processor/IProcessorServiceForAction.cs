using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common.Xrm;
using XRouter.Common;

namespace XRouter.Processor
{
    public interface IProcessorServiceForAction
    {
        void CreateMessage(Guid targetTokenGuid, string messageName, XDocument message);

        XDocument SendMessage(EndpointAddress target, XDocument message, XDocument metadata);

        XDocument GetXmlResource(XrmTarget target);
    }
}
