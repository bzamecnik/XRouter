using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator.ValueValidators;

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
        [ConfigurationItem("privateString", "description", "a default")]
        [RegexValidator("^a.*")]
        private string privateString;

        [ConfigurationItem("Integer", "Integer value expected.", 10)]
        [RangeValidator(1, 100)]
        public int Int { get; set; }

        [ConfigurationItem("Double", "description", 1.5d)]
        public double Double { get; private set; }

        [ConfigurationItem("enumValue", "description", EnumType.EnumValue2)]
        private EnumType enumValue;

        [ConfigurationItem("collectionOfNumbers", "description", new[] { 1, 2 })]
        [CountRangeValidator(2, 3)]
        private List<int> collectionOfNumbers;

        [ConfigurationItem("collectionOfCollections", "description", null)]
        private List<List<int>> collectionOfCollections;

        [ConfigurationItem("dictionary", "description", new object[] { 4, "4" })]
        private Dictionary<int, string> dictionary;

        public void SetValues()
        {
            privateString = "abc";
            Int = 11;
            Double = 99.5;

            collectionOfNumbers = new List<int>();
            collectionOfNumbers.Add(10);
            collectionOfNumbers.Add(11);

            dictionary = new Dictionary<int, string>();
            dictionary.Add(1, "one");
            dictionary.Add(2, "two");
        }

        public override string ToString()
        {
            return string.Format("privateString=\"{0}\"   Int={1}   Double={2}", privateString, Int, Double);
        }
    }
}
