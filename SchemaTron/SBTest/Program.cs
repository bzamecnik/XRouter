using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchemaTron;

namespace SBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            XDocument xSchema = XDocument.Load(@"C:\DipWork\schval\IN\sample.txt");
            Validator validator = Validator.Create(xSchema);
        }
    }
}
