using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Schematron.XsltValidator;
using Schematron.XsltValidator.App.Resources;

namespace Schematron.XsltValidator.App
{
    class Program
    {
        // sample

        static void Main(string[] args)
        {
            XDocument xSchema = Provider.LoadXmlDocument("sample_sch.xml");
            Validator validator = Validator.Create(xSchema);

            XDocument xDocument = Provider.LoadXmlDocument("sample_xml.xml");
            XDocument xResult = validator.Validate(xDocument);
            Console.WriteLine(xResult.ToString());
        }
    }
}
