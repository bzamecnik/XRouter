using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using CBR_prototype.Validator;

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

        private IValidatorAdapter[] validators = null;

        /// <summary>
        /// Creates a new CBR instance from a config file.
        /// </summary>
        /// <remarks>
        /// Prior to routing the validation predicates within CBR must be
        /// compiled using the Compile() method.</remarks>
        /// <param name="configFileName">File name of the configuration file.</param>
        /// <returns>a new CBR instance</returns>
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
        /// Compiles the predicate validators (from the respective schemas)
        /// with native Schematron validator implementation.
        /// </summary>
        public void Compile()
        {
            Compile(ValidatorImplementation.Native);
        }

        /// <summary>
        /// Compiles the predicate validators (from the respective schemas)
        /// with given Schematron validator implementation.
        /// </summary>
        public void Compile(ValidatorImplementation implementation)
        {
            List<IValidatorAdapter> validators = new List<IValidatorAdapter>();
            foreach (Predicate predicate in this.Predicate)
            {
                XDocument xSchema = XDocument.Load(predicate.SchematronPath,
                    LoadOptions.SetLineInfo);
                IValidatorAdapter validator = ValidatorFactory.CreateValidator(xSchema,
                    implementation);
                validators.Add(validator);
            }

            this.validators = validators.ToArray();
        }

        /// <summary>
        /// Metoda vrátí identifikátor predikátu. Pokud se nezdaří žádný predikát vybrat, pak 
        /// vrátí hodnotu default.
        /// </summary>
        /// <param name="xMessage"></param>
        /// <returns></returns>
        public String Route(XDocument xMessage)
        {
            if (validators == null)
            {
                throw new InvalidOperationException("The predicates are not yet compiled. "
                + "You must call Compile() prior to routing.");
            }

            for (Int32 i = 0; i < this.validators.Length; i++)
            {
                IValidatorAdapter validator = validators[i];
                bool isValid = validator.IsValid(xMessage, false);
                if (isValid)
                {
                    return this.Predicate[i].Id;
                }
            }

            return "default";
        }
    }
}
