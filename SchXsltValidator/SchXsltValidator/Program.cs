using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SchXsltValidator
{
    class Program
    {
        // sample

        static void Main(string[] args)
        {
            XDocument xSchema = XDocument.Load(@"..\..\sample_sch.xml", LoadOptions.SetLineInfo);
            Validator validator = Validator.Create(xSchema);
           
            XDocument xDoc = XDocument.Load(@"..\..\sample_sch.xml", LoadOptions.SetLineInfo);
            XDocument xResult = validator.Validate(xDoc);           
        }
    }
}
