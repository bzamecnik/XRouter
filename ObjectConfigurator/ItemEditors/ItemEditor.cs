using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ObjectConfigurator.ItemEditors
{
    abstract class ItemEditor
    {
        public ItemMetadata Metadata { get; private set; }

        public FrameworkElement Representation { get; protected set; }

        protected ItemEditor(ItemMetadata metadata)
        {
            Metadata = metadata;
        }

        public static ItemEditor CreateEditor(ItemMetadata metadata)
        {
            ItemEditor result;
            if (typeof(Enum).IsAssignableFrom(metadata.Type)) {
                result = new EnumItemEditor(metadata);
            } else {
                result = new TextItemEditor(metadata);
            }
            return result;
        }

        public abstract void SetValue(object value);
        public abstract object GetValue();
    }
}
