namespace SchemaTron.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using SchemaTron;
    using Xunit;

    public class ValidatorTest
    {
        private const string BASIC_SCHEMA = "basics_sch.xml";
        private const string BASIC_DOCUMENT = "basics_xml.xml";
        private const string PHASES_SCHEMA = "phases_sch.xml";

        [Fact]
        public void CreateValidatorNullSchemaNoSettings()
        {
            Assert.Throws<ArgumentNullException>(() => Validator.Create(null));
        }

        [Fact]
        public void CreateValidatorNullSchemaWithSettings()
        {
            Assert.Throws<ArgumentNullException>(() => Validator.Create(
                null, new ValidatorSettings()));
        }

        [Fact]
        public void CreateValidatorGoodSchemaWithoutSettings()
        {
            XDocument xSchema = Resources.Provider.LoadXmlDocument(BASIC_SCHEMA);
            Validator validator = Validator.Create(xSchema);
            Assert.NotNull(validator);
        }

        [Fact]
        public void CreateValidatorGoodSchemaWithSettings()
        {
            XDocument xSchema = Resources.Provider.LoadXmlDocument(BASIC_SCHEMA);
            Validator validator = Validator.Create(xSchema, new ValidatorSettings());
            Assert.NotNull(validator);
        }

        [Fact]
        public void CreateValidatorGoodSchemaWithCustomSettings()
        {
            XDocument xSchema = Resources.Provider.LoadXmlDocument(PHASES_SCHEMA);
            ValidatorSettings settings = new ValidatorSettings()
            {
                Phase = "#DEFAULT",
                InclusionsResolver = new CustomInclusionResolver()
            };
            Validator validator = Validator.Create(xSchema, settings);
            Assert.NotNull(validator);
        }

        [Fact]
        public void ValidateNullDocument()
        {
            XDocument xSchema = Resources.Provider.LoadXmlDocument(BASIC_SCHEMA);
            Validator validator = Validator.Create(xSchema);
            Assert.Throws<ArgumentNullException>(() => validator.Validate(null, true));
        }

        [Fact]
        public void ValidateGoodDocument()
        {
            XDocument xSchema = Resources.Provider.LoadXmlDocument(BASIC_SCHEMA);
            XDocument xDocument = Resources.Provider.LoadXmlDocument(BASIC_DOCUMENT);

            Validator validator = Validator.Create(xSchema);
            Assert.NotNull(validator);

            ValidatorResults results = validator.Validate(xDocument, true);

            Assert.True(results.IsValid);
        }

        public class CustomInclusionResolver : SchemaTron.IInclusionResolver
        {
            public XDocument Resolve(string href)
            {
                return Resources.Provider.LoadXmlDocument(href);
            }
        }
    }
}
