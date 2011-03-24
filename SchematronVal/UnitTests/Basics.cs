using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchematronVal;

namespace UnitTests
{
    public class Basics
    {
        public void SimpleValidation()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("basics_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("basics_xml.xml");

            Validator validator = Validator.Create(xSch);
            ValidatorResults results = validator.Validate(xIn, true);
        }

        public void SimpleValidation_InvalidInstance()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("basics_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("basics_xml_invalid.xml");

            Validator validator = Validator.Create(xSch);
            ValidatorResults results = validator.Validate(xIn, true);
        }
    }
}
