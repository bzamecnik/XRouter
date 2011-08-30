using System.Xml.Linq;

namespace ObjectConfigurator
{
    /// <summary>
    /// This interface must be implemented by custom configuration item types. 
    /// Implement this interface for extending support for new types of configurable items.
    /// </summary>
    public interface ICustomConfigurationItemType
    {
        /// <summary>
        /// Determines support for which type is added by this instance.
        /// </summary>
        /// <param name="typeFullName">Full name of type which will be tested for being supported by this instance.</param>
        /// <returns>True if type is supported.</returns>
        bool AcceptType(string typeFullName);

        /// <summary>
        /// Writes default value for custom type into a content of xml element.
        /// </summary>
        /// <param name="target">Target xml element.</param>
        void WriteDefaultValueToXElement(XElement target);

        /// <summary>
        /// Writes default value (specified by string) for custom type into a content of xml element.
        /// </summary>
        /// <param name="target">Target xml element.</param>
        /// <param name="defaultValue">Default value (represented as string) to write.</param>
        void WriteDefaultValueToXElement(XElement target, object defaultValue);

        /// <summary>
        /// Writes given value of supported type into a content of an xml element.
        /// </summary>
        /// <param name="target">Target xml element.</param>
        /// <param name="value">Value to be written in xml element.</param>
        void WriteToXElement(XElement target, object value);

        /// <summary>
        /// Reads a value of supported type from a content of an xml element.
        /// </summary>
        /// <param name="source">Source xml element.</param>
        /// <returns>Read value.</returns>
        object ReadFromXElement(XElement source);

        /// <summary>
        /// Creates custom WPF editor for supported type.
        /// </summary>
        /// <returns>An object providing interaction with WPF editor.</returns>
        ICustomConfigurationValueEditor CreateEditor();
    }
}
