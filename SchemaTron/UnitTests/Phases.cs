using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchemaTron;

namespace UnitTests
{
    public class Phases
    {
        public void SimpleValidation()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("phases_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("phases_xml.xml");

            // #ALL
            ValidatorSetting setting = new ValidatorSetting();
            setting.Phase = "#ALL";
            Validator validator = Validator.Create(xSch, setting);
            ValidatorResults results = validator.Validate(xIn, true);

            // #DEFAULT
            setting = new ValidatorSetting();
            setting.Phase = "#DEFAULT";
            validator = Validator.Create(xSch, setting);
            results = validator.Validate(xIn, true);

            // A
            setting = new ValidatorSetting();
            setting.Phase = "A";
            validator = Validator.Create(xSch, setting);
            results = validator.Validate(xIn, true);

            // B
            setting = new ValidatorSetting();
            setting.Phase = "B";
            validator = Validator.Create(xSch, setting);
            results = validator.Validate(xIn, true);
        }

        public void SimpleValidation_InvalidInstance()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("phases_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("phases_xml_invalid.xml");

            // #ALL
            ValidatorSetting setting = new ValidatorSetting();
            setting.Phase = "#ALL";
            Validator validator = Validator.Create(xSch, setting);
            ValidatorResults results = validator.Validate(xIn, true);

            // #DEFAULT
            setting = new ValidatorSetting();
            setting.Phase = "#DEFAULT";
            validator = Validator.Create(xSch, setting);
            results = validator.Validate(xIn, true);

            // A
            setting = new ValidatorSetting();
            setting.Phase = "A";
            validator = Validator.Create(xSch, setting);
            results = validator.Validate(xIn, true);

            // B
            setting = new ValidatorSetting();
            setting.Phase = "B";
            validator = Validator.Create(xSch, setting);
            results = validator.Validate(xIn, true);
        }
    }
}
