using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CBR_prototype.Validator
{
    internal class XsltValidatorAdapter : IValidatorAdapter
    {
        private Schematron.XsltValidator.Validator validator;

        private XmlNamespaceManager nsManager;

        public XsltValidatorAdapter(Schematron.XsltValidator.Validator validator)
        {
            this.validator = validator;
            this.nsManager = new XmlNamespaceManager(new NameTable());
            // Note: this should not be hard-coded but rather read from the
            // SVRL report
            this.nsManager.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");
        }

        public bool IsValid(XDocument xDocument, bool fullValidation)
        {
            XDocument svrlReport = validator.Validate(xDocument);
            // TODO: Find out from the SVRL report if the document was valid
            // and possibly the reasons of non-validity and fill them to the
            // SchemaTron.ValidatorResults.
            // Extract svrl:failed-assert/svrl:text and svrl:successful-report/svrl:text.

            IEnumerable<XElement> failedAsserts = svrlReport.XPathSelectElements(
                "/svrl:schematron-output/svrl:failed-assert|/svrl:schematron-output/svrl:successful-report", nsManager);
            bool isValid = failedAsserts.Count() == 0;
            return isValid;
        }
    }
}
