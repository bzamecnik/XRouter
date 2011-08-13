using System;
using System.Collections.Generic;
using XRouter.Common;

namespace XRouter.Broker
{
    /// <summary>
    /// API of the broker component to be used by dispatcher.
    /// </summary>
    /// <seealso cref="XRouter.Broker.Dispatching.Dispatcher"/>
    internal interface IBrokerServiceForDispatcher
    {
        /// <summary>
        /// Obtains the current application configuration.
        /// </summary>
        /// <returns>application configuration</returns>
        ApplicationConfiguration GetConfiguration();

        /// <summary>
        /// Obtains the collection of references pro processor components.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProcessorAccessor> GetProcessors();

        /// <summary>
        /// Obtains the collection of tokens being processed in a given
        /// processor.
        /// </summary>
        /// <param name="processorName">processor name</param>
        /// <returns></returns>
        IEnumerable<Token> GetActiveTokensAssignedToProcessor(string processorName);

        /// <summary>
        /// Obtains the collection of tokens which should be processed but are
        /// not assigned to any processor.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Token> GetUndispatchedTokens();

        /// <summary>
        /// Assigns token to a processor.
        /// </summary>
        /// <param name="tokenGuid">identifier of the token</param>
        /// <param name="assignedProcessor">processor name</param>
        void UpdateTokenAssignedProcessor(Guid tokenGuid, string assignedProcessor);

        /// <summary>
        /// Updates the message flow with which the token should be processed.
        /// </summary>
        /// <param name="tokenGuid">identifier of the token</param>
        /// <param name="messageFlowGuid">identifier of the message flow</param>
        void UpdateTokenMessageFlow(Guid tokenGuid, Guid messageFlowGuid);

        /// <summary>
        /// Updates the time of most recent time when the processor updated
        /// the token.
        /// </summary>
        /// <param name="tokenGuid">identifier of the token</param>
        /// <param name="lastResponse">time of the last processor response</param>
        void UpdateTokenLastResponseFromProcessor(Guid tokenGuid, DateTime lastResponse);

        /// <summary>
        /// Obtains the token identified by a GUID.
        /// </summary>
        /// <param name="tokenGuid">identifier of the token</param>
        /// <returns>token with the specified GUID or null if no such token
        /// exists</returns>
        Token GetToken(Guid tokenGuid);
    }
}
