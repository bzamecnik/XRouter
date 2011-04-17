using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBR_prototype.Validator
{
    /// <summary>
    /// Specifies the implementation of a Schematron validator.
    /// </summary>
    /// <seealso cref="IValidatorAdapter"/>
    /// <seealso cref="ValidatorFactory"/>
    public enum ValidatorImplementation
    {
        /// <summary>
        /// Native C# Schematron validator - SchemaTron.
        /// </summary>
        /// <seealso cref="NativeValidatorAdapter"/>
        Native,
        /// <summary>
        /// XSLT-based Schematron validator - Schematron.XsltValidator.
        /// </summary>
        /// <seealso cref="XsltValidatorAdapter"/>
        XSLT
    }
}
