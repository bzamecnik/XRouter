using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace Schematron.Benchmark
{
    /// <summary>
    /// Measures the performance of Schematron validators.
    /// </summary>
    /// <remarks>
    /// Measures the average time to create a validator from the schema and
    /// the time to validate an XML document.
    /// </remarks>
    class Program
    {
        private static readonly string DEFAULT_SCHEMA_FILE_NAME = @"XML\sample_sch.xml";
        private static readonly string DEFAULT_DOCUMENT_FILE_NAME = @"XML\sample_xml.xml";

        static void Main(string[] args)
        {

            XDocument xSchema;
            XDocument xDocument;
            PrepareXmlDocuments(args, out xSchema, out xDocument);

            Console.WriteLine("===== XSLT ISO Schematron validator =====");
            Console.WriteLine();
            MeasureXsltValidator(xSchema, xDocument);
            Console.WriteLine();

            Console.WriteLine("===== SchemaTron - native C# ISO Schematron validator =====");
            Console.WriteLine();
            MeasureNativeValidator(xSchema, xDocument);
        }

        /// <summary>
        /// Measures the sISO XSLT Schematron validator.
        /// </summary>
        /// <param name="xSchema"></param>
        /// <param name="xDocument"></param>
        private static void MeasureXsltValidator(XDocument xSchema, XDocument xDocument)
        {
            MeasureAction(() =>
            {
                Schematron.XsltValidator.Validator validator = Schematron.XsltValidator.Validator.Create(xSchema);
                validator.Validate(xDocument);
            }, 25, "Create a validator and validate");

            MeasureAction(() =>
            {
                Schematron.XsltValidator.Validator.Create(xSchema);
            }, 25, "Only create a validator");

            Schematron.XsltValidator.Validator xsltValidator = Schematron.XsltValidator.Validator.Create(xSchema);
            MeasureAction(() =>
            {
                xsltValidator.Validate(xDocument);
            }, 50000, "Only validate - full validation");
        }

        /// <summary>
        /// Measures SchemaTron, a native C# ISO Schematron validator.
        /// </summary>
        /// <param name="xSchema"></param>
        /// <param name="xDocument"></param>
        private static void MeasureNativeValidator(XDocument xSchema, XDocument xDocument)
        {
            MeasureAction(() =>
            {
                SchemaTron.Validator validator = SchemaTron.Validator.Create(xSchema);
            }, 500, "Only create a validator");

            SchemaTron.Validator nativeValidator = SchemaTron.Validator.Create(xSchema);
            MeasureAction(() =>
            {
                nativeValidator.Validate(xDocument, true);
            }, 50000, "Only validate - full validation");

            MeasureAction(() =>
            {
                nativeValidator.Validate(xDocument, false);
            }, 200000, "Only validate - partial validation");
        }

        /// <summary>
        /// Measures one action repeated in a loop in given numer of iterations.
        /// Report total time, average time, frequency and a comment on the
        /// action.
        /// </summary>
        /// <remarks>
        /// Note: Using lambda functions for representing the action doesn't
        /// have any measurable negative impact on performance compared to
        /// calling the action as a function inside the loop.
        /// </remarks>
        /// <param name="action">the action which performance to measure</param>
        /// <param name="iterationCount">number of iterations</param>
        /// <param name="comment">comment on the action</param>
        private static void MeasureAction(Action action, int iterationCount, string comment)
        {
            Console.WriteLine(comment);
            Console.Write("Number of iterations: {0}", iterationCount);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Reset();
            stopWatch.Start();

            for (int i = 0; i < iterationCount; i++)
            {
                action();
            }

            stopWatch.Stop();
            float avgTime = stopWatch.ElapsedMilliseconds / (float)iterationCount;
            Console.WriteLine(", Total time: {0} ms, Average time: {1} ms, Throughput: {2} / sec", stopWatch.ElapsedMilliseconds, avgTime, 1000 / avgTime);
            Console.WriteLine();
        }

        /// <summary>
        /// Loads the Schematron schema and the validated XML document.
        /// </summary>
        /// <remarks>
        /// Try to obtain the file names from command line arguments. If those
        /// are not available use default files.
        /// </remarks>
        /// <param name="args">command line arguments of the program</param>
        /// <param name="xSchema">Schematron schema</param>
        /// <param name="xDocument">validated XML document</param>
        private static void PrepareXmlDocuments(string[] args, out XDocument xSchema, out XDocument xDocument)
        {
            string schemaFileName = string.Empty;
            string documentFileName = string.Empty;
            if (args.Length == 0)
            {
                schemaFileName = DEFAULT_SCHEMA_FILE_NAME;
                documentFileName = DEFAULT_DOCUMENT_FILE_NAME;
            }
            else if (args.Length == 2)
            {
                schemaFileName = args[0];
                documentFileName = args[1];
            }
            else
            {
                Console.WriteLine("Usage: {0} [SCHEMA DOCUMENT]",
                    System.AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(1);
            }
            xSchema = XDocument.Load(schemaFileName);
            xDocument = XDocument.Load(documentFileName);
        }
    }
}
