using System;
using System.Xml.Linq;

namespace XRouter.Common.Xrm
{
    public interface IXmlStorage
    {
        event Action XmlChanged;

        void SaveXml(XDocument xml);
        XDocument LoadXml();
    }
}
