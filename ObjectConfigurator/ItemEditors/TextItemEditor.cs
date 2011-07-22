﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;

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

        public override void WriteToXElement(XElement target)
        {
            target.Value = uiText.Text;
        }

        public override void ReadFromXElement(XElement source)
        {
            uiText.Text = source.Value;
        }
    }
}
