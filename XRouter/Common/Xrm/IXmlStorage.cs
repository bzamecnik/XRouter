using System;
using System.Xml.Linq;

namespace XRouter.Common.Xrm
{
    /// <summary>
    /// Represents a XML storage with a single document which is able to
    /// notify when the contents has changed.
    /// </summary>
    public interface IXmlStorage
    {
        /// <summary>
        /// Stores the provided XML document into the storage replacing the
        /// possible existing one.
        /// </summary>
        /// <param name="xml">new XML document</param>
        void SaveXml(XDocument xml);

        /// <summary>
        /// Loads the XML document from the storage
        /// </summary>
        /// <returns>XML document stored in the storage; can be null</returns>
        XDocument LoadXml();
    }
}
