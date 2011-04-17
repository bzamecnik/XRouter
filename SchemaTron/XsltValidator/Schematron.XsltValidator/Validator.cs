using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Diagnostics;

namespace Schematron.XsltValidator
{
    public class Validator : SchemaTron.IValidator
    {
        private XslCompiledTransform xslTranform = null;

        private XmlNamespaceManager nsManager;

        public Validator()
        {
            nsManager = new XmlNamespaceManager(new NameTable());
            // Note: this should not be hard-coded but rather read from the
            // SVRL report
            nsManager.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");
        }

        /// <summary>
        /// Creates a XSTL-based ISO Schematron validator.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The purpose is to precompute the XSTL in order to validate then
        /// many XML documents using the same schema.
        /// </para>
        /// <para>
        /// The performance of this method in not that interesing.
        /// </para>
        /// </remarks>
        /// <param name="xSchema">Schematron schema for validation</param>
        /// <returns>Schematron validator with a precomputed XSLT transform</returns>
        public static Validator Create(XDocument xSchema)
        {
            Validator validator = new Validator();

            XslCompiledTransform xsl1 = LoadXsl("iso_dsdl_include.xsl");
            XslCompiledTransform xsl2 = LoadXsl("iso_abstract_expand.xsl");
            XslCompiledTransform xsl3 = LoadXsl("iso_svrl_for_xslt1.xsl");

            XmlDocument xDoc0 = new XmlDocument();
            xDoc0.LoadXml(xSchema.ToString());

            XmlDocument xDoc1 = Transform(xsl1, xDoc0);
            XmlDocument xDoc2 = Transform(xsl2, xDoc1);
            XmlDocument xDoc3 = Transform(xsl3, xDoc2);

            validator.xslTranform = new XslCompiledTransform();
            validator.xslTranform.Load(xDoc3);

            return validator;
        }

        private static Validator CreateWithMeasurement(XDocument xSchema)
        {
            Validator validator = new Validator();

            XslCompiledTransform xsl1 = LoadXsl("iso_dsdl_include.xsl");
            XslCompiledTransform xsl2 = LoadXsl("iso_abstract_expand.xsl");
            XslCompiledTransform xsl3 = LoadXsl("iso_svrl_for_xslt1.xsl");

            int iterationCount = 1000;
            Console.WriteLine("Action description: Create XSLT validator");
            Console.WriteLine("Number of iterations: {0}", iterationCount);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Reset();
            stopWatch.Start();

            for (int i = 0; i < iterationCount; i++)
            {
                XmlDocument xDoc0 = new XmlDocument();
                xDoc0.LoadXml(xSchema.ToString());

                XmlDocument xDoc1 = Transform(xsl1, xDoc0);
                XmlDocument xDoc2 = Transform(xsl2, xDoc1);
                XmlDocument xDoc3 = Transform(xsl3, xDoc2);

                validator.xslTranform = new XslCompiledTransform();
                validator.xslTranform.Load(xDoc3);

            }
            stopWatch.Stop();
            float avgTime = stopWatch.ElapsedMilliseconds / (float)iterationCount;
            Console.WriteLine("Total time: {0} ms", stopWatch.ElapsedMilliseconds);
            Console.WriteLine("Average time: {0} ms", avgTime);
            Console.WriteLine("Throughput: {0} / sec", 1000 / avgTime);
            Console.WriteLine();

            return validator;
        }

        /// <summary>
        /// Validates an XML document using the prepared XSTL-based ISO
        /// Schematron validator.
        /// </summary>
        /// <param name="xDocument">XML document to be validated</param>
        /// <param name="fullValidation">This parameter is ignored. Documents
        /// are always fully validated.</param>
        /// <returns>validation results</returns>
        public SchemaTron.ValidatorResults Validate(XDocument xDocument, bool fullValidation)
        {
            XDocument svrlReport = Validate(xDocument);
            // TODO: Find out from the SVRL report if the document was valid
            // and possibly the reasons of non-validity and fill them to the
            // ValidatorResults.
            // Extract svrl:failed-assert/svrl:text and svrl:successful-report/svrl:text.

            IEnumerable<XElement> failedAsserts = svrlReport.XPathSelectElements("/svrl:schematron-output/svrl:failed-assert|/svrl:schematron-output/svrl:successful-report", nsManager);

            SchemaTron.ValidatorResults results = new SchemaTron.ValidatorResults()
            {
                IsValid = failedAsserts.Count() == 0
                // ViolatedAssertions = ... // TODO
            };
            return results;
        }

        /// <summary>
        /// Validates an XML document using the prepared XSTL-based ISO
        /// Schematron validator.
        /// </summary>
        /// <remarks>
        /// We are primarily interested in performance of this method.
        /// </remarks>
        /// <param name="xDocument">XML document to be validated</param>
        /// <returns>SVRL report of the validation</returns>
        public XDocument Validate(XDocument xDocument)
        {
            // transformujeme do XDocumentu, pro jeho naslednou snadnou pouzitelnost

            XDocument transformedDoc = new XDocument();
            using (XmlWriter writer = transformedDoc.CreateWriter())
            {
                this.xslTranform.Transform(xDocument.CreateReader(), writer);
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

        private static XslCompiledTransform LoadXsl(String file)
        {
            XsltSettings settings = new XsltSettings(false, true);
            settings.EnableDocumentFunction = true;
            XslCompiledTransform xsl = new XslCompiledTransform();
            // TODO: XSL files could be loaded from embedded resources
            xsl.Load(file, settings, new XmlUrlResolver());
            return xsl;
        }

        private static XmlDocument Transform(XslCompiledTransform xsl, XmlDocument xDoc)
        {
            StringWriter sw = new StringWriter();
            // TODO: This might be inefficient. Instead of using TextWriter
            // and then creating XmlDocument from it, transform right into
            // XmlWriter. See:
            // http://stackoverflow.com/questions/1346995/how-to-create-a-xmldocument-using-xmlwriter-in-net
            // Or even better directly construct XDocument using the XmlWriter.
            xsl.Transform(xDoc, null, sw);
            XmlDocument result = new XmlDocument();
            result.LoadXml(sw.ToString());
            return result;
        }
    }
}
