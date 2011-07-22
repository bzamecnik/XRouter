using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemEditors
{
    class DictionaryItemEditor : ItemEditor
    {
        public DictionaryItemEditor(ItemMetadata metadata)
            : base(metadata)
        {
        }

        public override void WriteToXElement(XElement target)
        {
            throw new NotImplementedException();
        }

        public override void ReadFromXElement(XElement source)
        {
            throw new NotImplementedException();
        }
    }
}
