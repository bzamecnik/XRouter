using System;
using System.Xml.Linq;

namespace SchemaTron
{
    public interface IValidator
    {
        ValidatorResults Validate(XDocument xDocument, bool fullValidation);
    }
}
