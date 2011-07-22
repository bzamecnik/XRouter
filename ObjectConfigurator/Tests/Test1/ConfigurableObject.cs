using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectConfigurator.Tests.Test1
{
    enum EnumType
    {
        EnumValue1,
        EnumValue2,
        EnumValue3
    }

    class ConfigurableObject
    {
        [ConfigurationItem("privateString", "description", "default")]
        private string privateString;

        [ConfigurationItem("Integer", "Integer value expected.", 10)]
        public int Int { get; set; }

        [ConfigurationItem("Double", "description", 1.5d)]
        public double Double { get; private set; }

        [ConfigurationItem("enumValue", "description", EnumType.EnumValue2)]
        private EnumType enumValue;

        public void SetValues()
        {
            privateString = "abc";
            Int = 11;
            Double = 99.5;
        }

        public override string ToString()
        {
            return string.Format("privateString=\"{0}\"   Int={1}   Double={2}", privateString, Int, Double);
        }
    }
}
