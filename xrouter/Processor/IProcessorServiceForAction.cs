using System;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.Xrm;

namespace XRouter.Processor
{
    public interface IProcessorServiceForAction
    {
        void CreateMessage(Guid targetTokenGuid, string messageName, XDocument message);

        void AddExceptionToToken(Guid targetTokenGuid, Exception ex);

        XDocument SendMessage(EndpointAddress target, XDocument message, XDocument metadata = null);

        XDocument GetXmlResource(XrmUri target);
    }
}
