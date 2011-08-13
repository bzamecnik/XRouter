using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Prototype.CoreTypes;

namespace XRouter.Prototype.CoreServices
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
        </gateway>
        <processor name='processor1'>
        </processor>
    </components>
    <dispatcher nonRunningProcessorResponseTimeout='60'>
    </dispatcher>
    <workflows>
        <workflow version='1'>
        </workflow>
    <workflows>
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

        public Token[] GetReceivedTokens()
        {
            lock (storageLock) {
                Token[] result = InternalTokens.Where(t => t.State == TokenState.Received).ToArray();
                return result;
            }
        }

        public Token[] GetTokensAssignedToProcessor(string processorName)
        {
            lock (storageLock) {
                Token[] result = InternalTokens.Where(t => t.WorkflowState.AssignedProcessor == processorName).ToArray();
                return result;
            }
        }
    }
}
