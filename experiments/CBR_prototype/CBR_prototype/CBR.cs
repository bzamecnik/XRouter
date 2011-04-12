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

        /// <summary>
        /// Pouze načte konfiguraci.
        /// </summary>
        /// <returns></returns>
        public static CBR Deserialize(String fileName)
        {
            CBR instance = null;
            FileStream stream = null;
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(CBR));
            instance = (CBR)serializer.Deserialize(stream);
            stream.Close();
            return instance;
        }

        private Validator[] validators = null;

        /// <summary>
        /// Připraví si (zkompiluje) predikáty (Schematron schémata). 
        /// </summary>
        public void Compile()
        {
            List<Validator> vals = new List<Validator>();
            foreach (Predicate predicate in this.Predicate)
            {
                XDocument xSchema = XDocument.Load(predicate.SchematronPath, LoadOptions.SetLineInfo);
                try
                {
                    Validator val = Validator.Create(xSchema);
                    vals.Add(val);
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine("Chyba syntaxe Schematron.");
                    foreach (String s in e.UserMessages)
                    {
                        Console.WriteLine(s);
                    }
                    throw e;
                }
            }
            this.validators = vals.ToArray();
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
                Validator val = validators[i];
                ValidatorResults results = val.Validate(xMessage, false);
                if (results.IsValid)
                {
                    return this.Predicate[i].Id;
                }
            }

            return "default";
        }

    }
}
