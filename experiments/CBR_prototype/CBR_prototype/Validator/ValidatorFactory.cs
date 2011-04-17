using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CBR_prototype.Validator
{
    /// <summary>
    /// Validator factory enabled creating instances of a Schematron validator
    /// according to selected implementation. Each validator instance is
    /// wrapped into an IValidatorAdapter for CBR within CBR.
    /// </summary>
    /// <remarks>
    /// The implementation is selected via the ValidatorImplementation enum.
    /// </remarks>
    internal class ValidatorFactory
    {
        /// <summary>
        /// Create a new instance of a Schematron validator according to
        /// given implementation, wrapped into an IValidatorAdapter.
        /// </summary>
        /// <param name="xSchema">Schematron schema</param>
        /// <param name="implementation">Implementation of validator</param>
        /// <returns>A new validator instance</returns>
        /// <seealso cref="ValidatorImplementation"/>
        public static IValidatorAdapter CreateValidator(
            XDocument xSchema,
            ValidatorImplementation implementation)
        {
            switch (implementation)
            {
                case ValidatorImplementation.Native:
                    try
                    {
                        SchemaTron.Validator nativeValidator =
                            SchemaTron.Validator.Create(xSchema);
                        return new NativeValidatorAdapter(nativeValidator);
                    }
                    catch (SchemaTron.SyntaxException ex)
                    {
                        Console.WriteLine("Schematron syntax error:");
                        foreach (String message in ex.UserMessages)
                        {
                            Console.WriteLine(message);
                        }
                        throw ex;
                    }
                case ValidatorImplementation.XSLT:
                    Schematron.XsltValidator.Validator xsltValidator =
                        Schematron.XsltValidator.Validator.Create(xSchema);
                    return new XsltValidatorAdapter(xsltValidator);
                default:
                    throw new InvalidOperationException(string.Format(
                        "Unknown validator implementation: {0}",
                        implementation));
            }
        }
    }
}
