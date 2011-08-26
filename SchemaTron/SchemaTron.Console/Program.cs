using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SchemaTron.Console
{
    /// <summary>
    /// Console front-end of the SchemaTron validator.
    /// </summary>
    public class Program
    {
        string schemaFileName;
        string documentFileName;

        XDocument schema;
        XDocument document;

        string phase;

        bool fullValidationEnabled;
        bool areViolationsVerbose;
        bool isProcessingVerbose;

        static void Main(string[] args)
        {
            Program program = new Program();
            try
            {
                program.Run(args);
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine("Error: " + ex.Message);
            }
        }

        private void Run(string[] args)
        {
            ParseArguments(args);

            LoadFiles();

            Validator validator = CreateValidator(phase, schema);
            if (isProcessingVerbose)
            {
                System.Console.WriteLine(string.Format("Performing {0} validation.",
                    (fullValidationEnabled ? "full" : "partial")));
            }
            ValidatorResults results = validator.Validate(document, fullValidationEnabled);

            PrintResults(results);
        }

        private void LoadFiles()
        {
            if (isProcessingVerbose)
            {
                System.Console.WriteLine("Loading schema: " + schemaFileName);
            }
            schema = XDocument.Load(schemaFileName, LoadOptions.SetLineInfo);
            if (isProcessingVerbose)
            {
                System.Console.WriteLine("Loading document: " + documentFileName);
            }
            document = XDocument.Load(documentFileName, LoadOptions.SetLineInfo);
        }

        private Validator CreateValidator(string phase, XDocument schema)
        {
            ValidatorSettings validatorSettings = new ValidatorSettings();
            if (!string.IsNullOrWhiteSpace(phase))
            {
                validatorSettings.Phase = phase;
            }
            if (isProcessingVerbose)
            {
                System.Console.WriteLine(string.Format(
                    "Creating the validator with phase '{0}'.",
                    validatorSettings.Phase));
            }
            Validator validator = Validator.Create(schema, validatorSettings);
            return validator;
        }

        private void PrintResults(ValidatorResults results)
        {
            System.Console.WriteLine("Validation result: the document " + (results.IsValid ? "IS" : "is NOT") + " valid.");
            if (!results.IsValid)
            {
                System.Console.WriteLine(string.Format("Violated assertions ({0}):",
                    results.ViolatedAssertions.Count()));
                foreach (var assertion in results.ViolatedAssertions
                    .OrderBy((info) => info.LinePosition)
                    .OrderBy((info) => info.LineNumber))
                {
                    if (areViolationsVerbose)
                    {
                        System.Console.WriteLine(string.Format(
@"
User message: {0}
Line number: {2}, Line position: {3}
Assertion type: {1}
XPath location: {4}
Pattern id: '{5}'
Rule id: '{7}'
Rule context: {6}",
                        assertion.UserMessage,
                        assertion.IsReport ? "report" : "assert",
                        assertion.LineNumber,
                        assertion.LinePosition,
                        assertion.Location,
                        assertion.PatternId,
                        assertion.RuleContext,
                        assertion.RuleId));
                    }
                    else
                    {
                        System.Console.WriteLine(assertion.UserMessage);
                    }
                }
            }
        }

        private void ParseArguments(string[] args)
        {
            List<string> arguments = args.ToList();
            fullValidationEnabled = true;
            areViolationsVerbose = false;
            isProcessingVerbose = false;
            phase = null;

            string partialFlag = "-p";
            if (arguments.Contains(partialFlag))
            {
                fullValidationEnabled = false;
                arguments.Remove(partialFlag);
            }
            string verboseViolationsFlag = "-v";
            if (arguments.Contains(verboseViolationsFlag))
            {
                areViolationsVerbose = true;
                arguments.Remove(verboseViolationsFlag);
            }
            string verboseFlag = "-vv";
            if (arguments.Contains(verboseFlag))
            {
                areViolationsVerbose = true;
                isProcessingVerbose = true;
                arguments.Remove(verboseFlag);
            }
            phase = arguments.FirstOrDefault((arg) => arg.StartsWith("--phase="));
            if (phase != null)
            {
                phase = phase.Replace("--phase=", "");
            }

            if (arguments.Count < 2)
            {
                PrintUsage();
                Environment.Exit(-1);
            }
            schemaFileName = arguments[0];
            documentFileName = arguments[1];
        }

        private static void PrintUsage()
        {
            System.Console.WriteLine(
@"Usage: SchemaTron.Console.exe [options] SCHEMA DOCUMENT
SCHEMA - Schematron schema for validation
DOCUMENT - document to be validated
Options:
  -p - disables full validation (uses partial validation)
  -v - print more information on assertion violations
  -vv - be more vebose overall
  --phase=PHASE - select schema phase");
        }
    }
}
