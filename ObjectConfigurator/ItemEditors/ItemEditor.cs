using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

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
            if (metadata.Type is BasicItemType) {
                return new TextItemEditor(metadata);
            } 
            if (metadata.Type is EnumItemType) {
                return new EnumItemEditor(metadata);
            } 
            if (metadata.Type is CollectionItemType) {
                return new CollectionItemEditor(metadata);
            }
            if (metadata.Type is DictionaryItemType) {
                return new DictionaryItemEditor(metadata);
            }
            throw new InvalidOperationException("Unknown item type.");
        }

        public abstract void WriteToXElement(XElement target);

        public abstract void ReadFromXElement(XElement source);
    }
}
