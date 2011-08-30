using System;
using System.Windows;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    /// <summary>
    /// This interface describes interactions with a WPF editor. 
    /// It is suitable for WPF editor control to implement this interface directly althought it is not necessary.
    /// </summary>
    public interface ICustomConfigurationValueEditor
    {
        /// <summary>
        /// A value in GUI is changed by user.
        /// </summary>
        event Action ValueChanged;

        /// <summary>
        /// WPF element representing the editor itself.
        /// </summary>
        FrameworkElement Representation { get; }

        /// <summary>
        /// Writes a value from GUI into a content of an xml element.
        /// </summary>
        /// <param name="target">Target xml element.</param>
        /// <returns>True if value is valid and thus it is sucessfully written.</returns>
        bool WriteToXElement(XElement target);

        /// <summary>
        /// Reads and shows a value from a content of an xml element.
        /// </summary>
        /// <param name="source">Source xml element.</param>
        void ReadFromXElement(XElement source);
    }
}
