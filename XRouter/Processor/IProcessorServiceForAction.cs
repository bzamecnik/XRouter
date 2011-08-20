using System;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.Xrm;

namespace XRouter.Processor
{
    /// <summary>
    /// API of the processor component to be used by message flow actions.
    /// </summary>
    public interface IProcessorServiceForAction
    {
        /// <summary>
        /// Creates a new message and adds it to the token.
        /// </summary>
        /// <param name="targetToken">the token to be updated</param>
        /// <param name="messageName">name of the new message</param>
        /// <param name="message">contents of the new message</param>
        /// <param name="updatedToken">updated token</param>
        void CreateMessage(Token targetToken, string messageName, XDocument message);

        /// <summary>
        /// Adds an exception to the token.
        /// </summary>
        /// <param name="targetToken">token to be updated</param>
        /// <param name="exception">exception to be added</param>
        void AddExceptionToToken(Token targetToken, Exception exception);

        /// <summary>
        /// Sends an output message to a specified output endpoint -
        /// synchronously.
        /// </summary>
        /// <param name="address">output endpoint address</param>
        /// <param name="message">output message contents</param>
        /// <param name="metadata">optional output message metadata; can be null</param>
        /// <returns>optional reply message or null if there is no reply
        /// from the output endpoint</returns>
        XDocument SendMessage(EndpointAddress target, XDocument message, XDocument metadata = null);

        /// <summary>
        /// Obtains a XML resource identified its XRM URI from a XML resource
        /// storage.
        /// </summary>
        /// <param name="target">XRM URI of the requested resource</param>
        /// <returns>the requested resource XML resource; or null if none was
        /// found</returns>
        /// <seealso cref="XRouter.Common.Xrm.XmlResourceManager"/>
        XDocument GetXmlResource(XrmUri target);
    }
}
