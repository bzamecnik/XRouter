using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CBR_prototype.Validator
{
    internal class NativeValidatorAdapter : IValidatorAdapter
    {
        private SchemaTron.Validator validator;

        public NativeValidatorAdapter(SchemaTron.Validator validator)
        {
            this.validator = validator;
        }

        public bool IsValid(XDocument xDocument, bool fullValidation)
        {
            SchemaTron.ValidatorResults results = validator.Validate(xDocument, fullValidation);
            return results.IsValid;
        }
    }
}
