using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;

namespace SchXsltValidator
{
    class Validator
    {
        private XslCompiledTransform tranform = null;

        /// <summary>
        /// Rychlost teto metody neni zajimava. Jedna se o kesovani. 
        /// </summary>
        /// <param name="xSchema"></param>
        /// <returns></returns>
        public static Validator Create(XDocument xSchema)
        {
            Validator validator = new Validator();

            XslCompiledTransform xsl1 = LoadXsl(@"..\..\iso_dsdl_include.xsl");
            XslCompiledTransform xsl2 = LoadXsl(@"..\..\iso_abstract_expand.xsl");
            XslCompiledTransform xsl3 = LoadXsl(@"..\..\iso_svrl_for_xslt1.xsl");

            XmlDocument xDoc0 = new XmlDocument();
            xDoc0.LoadXml(xSchema.ToString());

            XmlDocument xDoc1 = Transform(xsl1, xDoc0);
            XmlDocument xDoc2 = Transform(xsl2, xDoc1);
            XmlDocument xDoc3 = Transform(xsl3, xDoc2);

            validator.tranform = new XslCompiledTransform();
            validator.tranform.Load(xDoc3);

            return validator;
        }

        private static XslCompiledTransform LoadXsl(String file)
        {
            XsltSettings settings = new XsltSettings(false, true);
            settings.EnableDocumentFunction = true;
            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(file, settings, new XmlUrlResolver());            
            return xsl;
        }

        private static XmlDocument Transform(XslCompiledTransform xsl, XmlDocument xDoc)
        {
            StringWriter sw = new StringWriter();
            xsl.Transform(xDoc, null, sw);
            XmlDocument result = new XmlDocument();
            result.LoadXml(sw.ToString());
            return result;
        }

        /// <summary>
        /// Zajima nas rychlost teto metody.
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public XDocument Validate(XDocument xDoc)
        {                     
            // transformujeme do XDocumentu, pro jeho naslednou snadnou pouzitelnost
                       
            XDocument transformedDoc = new XDocument();          
            using (XmlWriter writer = transformedDoc.CreateWriter())
            {
                this.tranform.Transform(xDoc.CreateReader(), writer);                
            }
            
            // aby to bylo pouzitelne, musel by se tady jeste zpracovat vysledny XML dokument a ziskat 
            // z nej treba pomoci XPathu diagnostiky validace, tedy neco jako //svrl:failed-assert

            // mozna by se dalo najit reseni, ze by se to transformovalo do neceho jineho, nez 
            // instance XDocumentu a z toho neceho jineho by se ziskaly vysledky a transformace by 
            // tak byla o kapku rychlejsi, vzhledem k tomu, ze je vsak XDocument dost optimalizovana XML
            // mezi-pamet (oproti treba starsimu XmlDocument), tak se domnivam, ze by to na vysledku
            // prilis nezmenilo (jestli vubec)

            return transformedDoc;
        }
    }
}
