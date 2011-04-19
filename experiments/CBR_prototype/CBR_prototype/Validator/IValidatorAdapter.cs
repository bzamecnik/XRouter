using System;
using System.Xml.Linq;

namespace CBR_prototype.Validator
{
    /// <summary>
    /// Schematron validator adapter.
    /// Enables CBR to utilize various Schematron validator implementations.
    /// </summary>
    internal interface IValidatorAdapter
    {
        bool IsValid(XDocument xDocument, bool fullValidation);
    }
}
