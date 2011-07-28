using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    public interface ICustomConfigurationValueEditor
    {
        event Action ValueChanged;

        FrameworkElement Representation { get; }

        bool WriteToXElement(XElement target);
        void ReadFromXElement(XElement source);
    }
}
