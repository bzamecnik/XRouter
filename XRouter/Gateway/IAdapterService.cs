using System;
using System.Xml.Linq;

namespace XRouter.Gateway
{
    /// <summary>
    /// A delegate which is performed when the token is being finished and
    /// there is a reply message to the original message.
    /// </summary>
    /// <param name="tokenGuid">identifier of the token</param>
    /// <param name="resultMessage">reply to the original message</param>
    /// <param name="sourceMetadata">metadata about the original message
    /// </param>
    /// <param name="context">an arbitrary context which needs to be stored
    /// from the time of the receiving the original message till the
    /// invocation of this handler</param>
    public delegate void MessageResultHandler(
        Guid tokenGuid,
        XDocument resultMessage,
        XDocument sourceMetadata,
        object context);

    // TODO: unused!
	public interface IAdapterService
	{
        void ReceiveMessage(
            XDocument message,
            string endpointName,
            XDocument metadata,
            object context,
            MessageResultHandler resultHandler);
	}
}
