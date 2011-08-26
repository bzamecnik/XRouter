using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchemaTron;

namespace XRouter.Gui.Xrm.DocumentTypeDescriptors
{
    class SchematronDocumentTypeDescriptor : XDocumentTypeDescriptor
    {
        public override string DocumentTypeName { get { return "Schematron"; } }

        public override XElement CreateDefaultRoot()
        {
            return XElement.Parse(@"<schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'>

</schema>", LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }

        public override bool IsValid(XDocument xDocument, out string errorDescription)
        {
            try {
                Validator validator = Validator.Create(xDocument);
                errorDescription = null;
                return true;
            } catch (SyntaxException ex) {
                errorDescription = string.Join(Environment.NewLine, ex.UserMessages);
                return false;
            }
        }

        public override System.Windows.Media.ImageSource GetIconSource()
        {
            return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/XRouter.GUI;component/Resources/text_xml.png"));
        }
    }
}
