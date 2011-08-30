using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XRouter.Gui.Xrm.DocumentTypeDescriptors
{
    class GeneralXDocumentTypeDescriptor : XDocumentTypeDescriptor
    {
        public override string DocumentTypeName { get { return "General xml"; } }

        public override XElement CreateDefaultRoot()
        {
            return XElement.Parse(@"<root>

</root>", LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }

        public override bool IsValid(XDocument xDocument, out string errorDescription)
        {
            errorDescription = null;
            return true;
        }

        public override System.Windows.Media.ImageSource GetIconSource()
        {
            return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/XRouter.GUI;component/Resources/xml.png"));
        }
    }
}
