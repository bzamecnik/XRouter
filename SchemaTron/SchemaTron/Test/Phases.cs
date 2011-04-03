using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchemaTron;

namespace SchemaTron.Test
{
    public class Phases
    {
        public void SimpleValidation()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("phases_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("phases_xml.xml");

            // #ALL
            ValidatorSettings settings = new ValidatorSettings();
            settings.Phase = "#ALL";
            Validator validator = Validator.Create(xSch, settings);
            ValidatorResults results = validator.Validate(xIn, true);

            // #DEFAULT
            settings = new ValidatorSettings();
            settings.Phase = "#DEFAULT";
            validator = Validator.Create(xSch, settings);
            results = validator.Validate(xIn, true);

            // A
            settings = new ValidatorSettings();
            settings.Phase = "A";
            validator = Validator.Create(xSch, settings);
            results = validator.Validate(xIn, true);

            // B
            settings = new ValidatorSettings();
            settings.Phase = "B";
            validator = Validator.Create(xSch, settings);
            results = validator.Validate(xIn, true);
        }

        public void SimpleValidation_InvalidInstance()
        {
            XDocument xSch = Resources.Provider.LoadXmlDocument("phases_sch.xml");
            XDocument xIn = Resources.Provider.LoadXmlDocument("phases_xml_invalid.xml");

            // #ALL
            ValidatorSettings settings = new ValidatorSettings();
            settings.Phase = "#ALL";
            Validator validator = Validator.Create(xSch, settings);
            ValidatorResults results = validator.Validate(xIn, true);

            // #DEFAULT
            settings = new ValidatorSettings();
            settings.Phase = "#DEFAULT";
            validator = Validator.Create(xSch, settings);
            results = validator.Validate(xIn, true);

            // A
            settings = new ValidatorSettings();
            settings.Phase = "A";
            validator = Validator.Create(xSch, settings);
            results = validator.Validate(xIn, true);

            // B
            settings = new ValidatorSettings();
            settings.Phase = "B";
            validator = Validator.Create(xSch, settings);
            results = validator.Validate(xIn, true);
        }
    }
}
