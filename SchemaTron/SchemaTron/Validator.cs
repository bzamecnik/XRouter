using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using SchemaTron.SyntaxModel;
using SchemaTron.Preprocessing;

namespace SchemaTron
{
    /// <summary>
    /// The native ISO Schematron validator over XPath 1.0 query language binding.
    /// </summary>
    public sealed class Validator
    {            
        /// <summary>
        /// Gets adjusted schema syntax.
        /// </summary>
        public XDocument MinSyntax { private set; get; }

        private Schema schema = null;

        private Validator()
        { }

        /// <summary>
        /// Vytvori novy SchematronVal.Validator z dane System.Xml.Linq.XDocument s vychozim 
        /// nastavenim validatoru. 
        /// </summary>
        /// <param name="xSchema">ISO Schematron complex syntax schema.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="ISOSchematronValidator.SyntaxException"/>
        public static Validator Create(XDocument xSchema)
        {
            return Create(xSchema, null);
        }

        /// <summary>
        /// Vytvori novy SchematronVal.Validator z dane System.Xml.Linq.XDocument.
        /// </summary>
        /// <param name="xSchema">ISO Schematron complex syntax schema.</param>
        /// <param name="setting">Konfigurace validatoru.</param>
        /// <returns></returns>     
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="ISOSchematronValidator.SyntaxException"/>
        public static Validator Create(XDocument xSchema, ValidatorSetting setting)
        {
            if (xSchema == null)
            {
                throw new ArgumentNullException("xSchema");
            }
            if (setting == null)
            {
                setting = new ValidatorSetting();
            }
            if (setting.InclusionsResolver == null)
            {
                setting.InclusionsResolver = new InclusionsFileResolver();
            }

            Validator validator = null;

            // make a deep copy of the supplied XML 
            XDocument xSchemaCopy = new XDocument(xSchema);

            // resolve iso namespace
            XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
            nsManager.AddNamespace("sch", Consts.ISONamespace);

            setting.Phase = DetermineSchemaPhase(xSchemaCopy.Root, setting.Phase, nsManager);
          
            // preprocessing
            Preprocessing(xSchemaCopy, nsManager, setting);
       
            // deserialization                           
            Schema schema = SchemaDeserializer.Deserialize(xSchemaCopy, nsManager);

            // xpath preprocessing
            CompileXPathExpressions(schema);

            // create instance
            validator = new Validator();
            validator.schema = schema;
            validator.MinSyntax = xSchemaCopy;

            return validator;
        }
               
        private static String DetermineSchemaPhase(XElement xRoot, String phase, XmlNamespaceManager nsManager)
        {           
            if (phase == null)
            {
                throw new ArgumentNullException("Phase");
            }
            else
                if (phase == "#ALL")
                {
                    return phase;
                }
                else
                    if (phase == "#DEFAULT")
                    {
                        XAttribute xPhase = xRoot.Attribute(XName.Get("defaultPhase"));
                        if (xPhase != null)
                        {
                            return xPhase.Value;
                        }
                        else
                        {
                            throw new ArgumentException("@defaultPhase is not specified.", "Phase");
                        }
                    }
                    else
                    {
                        if (xRoot.XPathSelectElement(String.Format("/sch:schema/sch:phase[@id='{0}']", phase), nsManager) != null)
                        {
                            return phase;
                        }
                        else
                        {
                            throw new ArgumentException(String.Format("Phase[@id='{0}'] is not specified.", phase), "Phase");
                        }
                    }
        }

        private static void Preprocessing(XDocument xSchema, XmlNamespaceManager nsManager, ValidatorSetting setting)
        {
            if (setting.Preprocessing)
            {
                ValidatorSetting valArgs = new ValidatorSetting();
                valArgs.Preprocessing = false;
                
                // validation - phaseA 
                XDocument xPhaseA = Resources.Provider.SchemaPhaseA;
                Validator validatorPhaseA = Validator.Create(xPhaseA, valArgs);
                ValidatorResults resultsA = validatorPhaseA.Validate(xSchema, true);
                if (!resultsA.IsValid)
                {
                    throw new SyntaxException(resultsA.GetMessages());
                }

                Preprocessor.ResolveInclusions(xSchema, setting.InclusionsResolver, nsManager);

                // validation - phaseB 
                XDocument xPhaseB = Resources.Provider.SchemaPhaseB;
                Validator validatorPhaseB = Validator.Create(xPhaseB, valArgs);
                ValidatorResults resultsB = validatorPhaseB.Validate(xSchema, true);
                if (!resultsB.IsValid)
                {
                    throw new SyntaxException(resultsB.GetMessages());
                }

                Preprocessor.ResolveAbstractPatterns(xSchema, nsManager);
                Preprocessor.ResolveAbstractRules(xSchema, nsManager);
                Preprocessor.ResolvePhase(xSchema, nsManager, setting.Phase);
               
                // validation - phaseC 
                XDocument xPhaseC = Resources.Provider.SchemaPhaseC;
                Validator validatorPhaseC = Validator.Create(xPhaseC, valArgs);
                ValidatorResults resultsC = validatorPhaseC.Validate(xSchema, true);
                if (!resultsC.IsValid)
                {                   
                    throw new SyntaxException(resultsC.GetMessages());
                }

                Preprocessor.ResolveLets(xSchema, nsManager);
                Preprocessor.ResolveAncillaryElements(xSchema, nsManager);
            }
        }
                        
        private static void CompileXPathExpressions(Schema schema)
        {
            List<String> messages = new List<String>();

            // resolve namespaces
            XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
            foreach (Ns ns in schema.Namespaces)
            {
                nsManager.AddNamespace(ns.Prefix, ns.Uri);
            }

            // compile XPath expressions
            foreach (Pattern pattern in schema.Patterns)
            {
                // compile contexts
                foreach (Rule rule in pattern.Rules)
                {
                    // alter xpath context
                    String context = rule.Context;
                    if (context.Length > 0 && context[0] != '/')
                    {
                        context = String.Concat("//", context);
                    }
                   
                    try
                    {
                        rule.CompiledContext = System.Xml.XPath.XPathExpression.Compile(context, nsManager);  
                    }
                    catch (XPathException e)
                    {
                        messages.Add(String.Format("Invalid XPath 1.0 context='{0}': {1}", rule.Context, e.Message));
                    }

                    // compile tests
                    foreach (Assert assert in rule.Asserts)
                    {
                        try
                        {
                            assert.CompiledTest = System.Xml.XPath.XPathExpression.Compile(assert.Test, nsManager);
                        }
                        catch (XPathException e)
                        {
                            messages.Add(String.Format("Invalid XPath 1.0 test='{0}': {1}", assert.Test, e.Message));
                        }

                        // compile diagnostics
                        if (assert.Diagnostics.Length > 0)
                        {                            
                            assert.CompiledDiagnostics = new XPathExpression[assert.Diagnostics.Length];
                            for (Int32 i = 0; i < assert.Diagnostics.Length; i++)
                            {
                                String diag = assert.Diagnostics[i];
                                try
                                {
                                    assert.CompiledDiagnostics[i] = XPathExpression.Compile(diag);
                                }
                                catch (XPathException e)
                                {
                                    if (assert.DiagnosticsIsValueOf[i])
                                    {
                                        messages.Add(String.Format("Invalid XPath 1.0 select='{0}': {1}", diag, e.Message));
                                    }
                                    else
                                    {
                                        messages.Add(String.Format("Invalid XPath 1.0 path='{0}']: {1}", diag, e.Message));
                                    }
                                }
                            }                                                       
                        }
                    }
                }               
            }

            // syntax errors
            if (messages.Count > 0)
            {
                throw new SyntaxException(messages.ToArray());
            }
        }
        
        /// <summary>
        /// Validacni funkce, ktera ocekava XML instanci jako System.Xml.Linq.XDocument. 
        /// </summary>
        /// <param name="xInstance">XML instance (pro lepsi diagnostiky je doporuceno dodat instanci s line information).</param>
        /// <param name="fullyValidation">Urcuje, jestli se ma validace po prvni vyvolane assertion prerusit.</param>
        /// <returns>Detailni vysledek validace.</returns>
        /// <remarks>Metoda neni thread safe.</remarks>
        public ValidatorResults Validate(XDocument xInstance, Boolean fullyValidation)
        {
            ValidatorResults results = null;
            ValidationEvaluator evaluator = new ValidationEvaluator(this.schema, xInstance, fullyValidation);
            results = evaluator.Evaulate();
            return results;            
        }                                               
    }
}
