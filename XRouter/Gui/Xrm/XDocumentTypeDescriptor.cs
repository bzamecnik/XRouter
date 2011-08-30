using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Media;

namespace XRouter.Gui.Xrm
{
    abstract class XDocumentTypeDescriptor
    {
        public abstract string DocumentTypeName { get; }

        public abstract XElement CreateDefaultRoot();

        public abstract bool IsValid(XDocument xDocument, out string errorDescription);

        public abstract ImageSource GetIconSource();
    }
}
