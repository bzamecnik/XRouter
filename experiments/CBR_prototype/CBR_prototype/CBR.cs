using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using SchemaTron;

namespace CBR_prototype
{
    /// <summary>
    /// Content-based XML message router prototype.
    /// </summary>
    [XmlRoot("cbr")]
    public class CBR
    {
        [XmlElement("predicate")]
        public Predicate[] Predicate = null;

        private IValidator[] validators = null;

        public ValidatorImplementationType ValidatorImplementation { get; set; }

        public CBR()
        {
            ValidatorImplementation = ValidatorImplementationType.Native;
        }

        /// <summary>
        /// Creates a new CBR instance from a config file.
        /// </summary>
        /// <param name="configFileName">File name of the configuration file.</param>
        /// <returns></returns>
        public static CBR Deserialize(String configFileName)
        {
            using (FileStream stream = new FileStream(configFileName,
                FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CBR));
                return (CBR)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Připraví si (zkompiluje) predikáty (Schematron schémata). 
        /// </summary>
        public void Compile()
        {
            List<IValidator> validators = new List<IValidator>();
            foreach (Predicate predicate in this.Predicate)
            {
                XDocument xSchema = XDocument.Load(predicate.SchematronPath,
                    LoadOptions.SetLineInfo);
                try
                {
                    IValidator validator = CreateValidator(xSchema);
                    validators.Add(validator);
                }
                catch (SyntaxException ex)
                {
                    Console.WriteLine("Schematron syntax error:");
                    foreach (String message in ex.UserMessages)
                    {
                        Console.WriteLine(message);
                    }
                    throw ex;
                }
            }
            this.validators = validators.ToArray();
        }

        private IValidator CreateValidator(XDocument xSchema)
        {
            switch (ValidatorImplementation)
            {
                case ValidatorImplementationType.Native:
                    return Validator.Create(xSchema);
                case ValidatorImplementationType.XSLT:
                    return Schematron.XsltValidator.Validator.Create(xSchema);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Metoda vrátí identifikátor predikátu. Pokud se nezdaří žádný predikát vybrat, pak 
        /// vrátí hodnotu default.
        /// </summary>
        /// <param name="xMessage"></param>
        /// <returns></returns>
        public String Route(XDocument xMessage)
        {
            for (Int32 i = 0; i < this.validators.Length; i++)
            {
                IValidator validator = validators[i];
                ValidatorResults results = validator.Validate(xMessage, false);
                if (results.IsValid)
                {
                    return this.Predicate[i].Id;
                }
            }

            return "default";
        }

        public enum ValidatorImplementationType
        {
            Native,
            XSLT
        }
    }
}
