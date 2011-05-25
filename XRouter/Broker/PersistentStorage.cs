using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common;

namespace XRouter.Broker
{
    class PersistentStorage
    {
        private object storageLock = new object();

        private XDocument configurationXml;

        private List<Token> InternalTokens { get; set; }

        public PersistentStorage()
        {
            configurationXml = XDocument.Parse(@"
<configuration>
    <components>
        <gateway name='gateway1'>
            <adapters>
                <adapter name='directoryAdapter' type='XRouter.Adapters.DirectoryAdapter,..\..\..\..\xrouter\Adapters\bin\debug\XRouter.Adapters.dll'>
                </adapter>
            </adapters>
        </gateway>
        <processor name='processor1' concurrent-threads='4'>
        </processor>
    </components>
    <dispatcher nonrunning-processor-response-timeout='60'>
    </dispatcher>
    <messageFlows current='messageflow-guid-1'>
        <messageFlow guid='messageflow-guid-1' name='abc' version='1'>
        </messageFlow>
    </messageFlows>
</configuration>
");
            InternalTokens = new List<Token>();
        }

        public XDocument GetConfigXml()
        {
            lock (storageLock) {
                return configurationXml;
            }
        }

        public void SaveConfigXml(XDocument configXml)
        {
            lock (storageLock) {
                this.configurationXml = configXml;
            }
        }

        public void SaveToken(Token token)
        {
            lock (storageLock) {
                Token oldToken = InternalTokens.FirstOrDefault(t => t.Guid == token.Guid);
                if (oldToken != null) {
                    InternalTokens.Remove(oldToken);
                }

                InternalTokens.Add(token.Clone());
            }
        }

        public void UpdateToken(Guid tokenGuid, Action<Token> updater)
        {
            lock (storageLock) {
                Token token = InternalTokens.First(t => t.Guid == tokenGuid);
                updater(token);
            }
        }

        public Token GetToken(Guid tokenGuid)
        {
            Token token = InternalTokens.First(t => t.Guid == tokenGuid);
            return token;
        }

        public IEnumerable<Token> GetUndispatchedTokens()
        {
            lock (storageLock) {
                Token[] result = InternalTokens.Where(t => t.MessageFlowState.AssignedProcessor == null).ToArray();
                return result;
            }
        }

        public IEnumerable<Token> GetActiveTokensAssignedToProcessor(string processorName)
        {
            lock (storageLock) {
                Token[] result = InternalTokens.Where(t => (t.MessageFlowState.AssignedProcessor == processorName) && (t.State == TokenState.InProcessor)).ToArray();
                return result;
            }
        }

        public IEnumerable<Guid> GetActiveMessageFlowsGuids()
        {
            lock (storageLock) {
                Guid[] result = InternalTokens.Where(t => t.State != TokenState.Finished).Select(t => t.MessageFlowState.MessageFlowGuid).Distinct().ToArray();
                return result;
            }
        }
    }
}
