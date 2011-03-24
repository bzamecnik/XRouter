using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchematronVal;

namespace UnitTests
{
    public class AbsPatterns
    {
        public void SimpleValidation()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("abspatterns_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("abspatterns_xml.xml");

            Validator validator = Validator.Create(xSch);
            ValidatorResults results = validator.Validate(xIn, true);
        }

        public void SimpleValidation_InvalidInstance()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("abspatterns_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("abspatterns_xml_invalid.xml");

            Validator validator = Validator.Create(xSch);
            ValidatorResults results = validator.Validate(xIn, true);
        }
    }
}
