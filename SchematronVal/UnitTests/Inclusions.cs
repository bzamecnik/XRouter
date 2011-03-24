using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchematronVal;

namespace UnitTests
{
    public class Inclusions
    {
        class InclusionsEmbededResourceResolver : SchematronVal.InclusionsResolver
        {
            public XDocument Resolve(String href)
            {
                return Resources.Provider.LoadXmlDocument(href); 
            }
        }

        public void SimpleValidation()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("inclusions_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("inclusions_xml.xml");

            ValidatorSetting setting = new ValidatorSetting();
            setting.InclusionsResolver = new InclusionsEmbededResourceResolver();
            Validator validator = Validator.Create(xSch, setting);
            ValidatorResults results = validator.Validate(xIn, true); 
        }

        public void SimpleValidation_InvalidInstance()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("inclusions_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("inclusions_xml_invalid.xml");

            ValidatorSetting setting = new ValidatorSetting();
            setting.InclusionsResolver = new InclusionsEmbededResourceResolver();
            Validator validator = Validator.Create(xSch, setting);

            // fully validation
            ValidatorResults results1 = validator.Validate(xIn, true);
            
            // not fully validation
            ValidatorResults results2 = validator.Validate(xIn, false);
        } 
    }
}
