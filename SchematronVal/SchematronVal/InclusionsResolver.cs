using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchematronVal
{
    /// <summary>
    /// Poskytuje rozhrani pro specifickou implementaci dodani included Schematron elementu. 
    /// </summary>
    public interface InclusionsResolver
    {
        /// <summary>
        /// Preprocessor vola tuto metodu pro kazdy vyskyt elementu include.
        /// </summary>
        /// <param name="href">Atribut obsahuje odkaz na externi well-formed XML dokument.</param>
        /// <returns></returns>
        XDocument Resolve(String href);
    }
}
