using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
