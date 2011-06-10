using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ObjectConfigurator.ItemEditors
{
    class TextItemEditor : ItemEditor
    {
        private TextBox uiText;

        public TextItemEditor(ItemMetadata metadata)
            : base(metadata)
        {
            uiText = new TextBox();
            Representation = uiText;
        }

        public override void SetValue(object value)
        {
            uiText.Text = Metadata.ToString(value);
        }

        public override object GetValue()
        {
            object result = Metadata.Parse(uiText.Text);
            return result;
        }
    }
}
