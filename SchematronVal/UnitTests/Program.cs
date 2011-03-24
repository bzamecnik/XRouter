using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchematronVal;

namespace UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
              Tady je několik hlavních případů užití. Cílem to není rozbít, spíše
              ukázat použití API. 
            */

            Basics basics = new Basics();
            basics.SimpleValidation();
            basics.SimpleValidation_InvalidInstance();

            Inclusions inclusions = new Inclusions();
            inclusions.SimpleValidation();
            inclusions.SimpleValidation_InvalidInstance();

            AbsPatterns absPatterns = new AbsPatterns();
            absPatterns.SimpleValidation();
            absPatterns.SimpleValidation_InvalidInstance();

            AbsRules absRules = new AbsRules();
            absRules.SimpleValidation();
            absRules.SimpleValidation_InvalidInstance();

            Phases phases = new Phases();
            phases.SimpleValidation();
            phases.SimpleValidation_InvalidInstance();
        }
    }
}
