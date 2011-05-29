using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common;
using XRouter.Common.Xrm;

namespace XRouter.Broker
{
    class PersistentStorage : IXmlStorage
    {
        private object storageLock = new object();

        private XDocument configurationXml;

        private List<Token> InternalTokens { get; set; }

        public event Action ConfigurationChanged = delegate { };

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
    <messageflows current='bcf9e0dd-818d-492c-84b6-4c27ca668221'>
        <messageflow guid='bcf9e0dd-818d-492c-84b6-4c27ca668221' name='abc' version='1'>
        </messageflow>
    </messageflows>
    <xml-resource-storage>

         <item name='containsA'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron'>
                <pattern id='containsText'>
                    <rule context='/content'>
                        <assert test=""/content[contains(., 'A')]"" />
                    </rule>
              </pattern>
            </schema>
         </item>

        <item name='containsB'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron'>
                <pattern id='containsText'>
                    <rule context='/content'>
                        <assert test=""/content[contains(., 'B')]"" />
                    </rule>
              </pattern>
            </schema>
         </item>

        <item name='containsC'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron'>
                <pattern id='containsText'>
                    <rule context='/content'>
                        <assert test=""/content[contains(., 'C')]"" />
                    </rule>
              </pattern>
            </schema>
         </item>

        <item name='xslt1'>
            <xsl:transform version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
                <xsl:template match='/'>
                    <content>Original content: <xsl:value-of select='.'/></content>
                </xsl:template>
            </xsl:transform>
         </item>

    </xml-resource-storage>
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
            ConfigurationChanged();
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

        #region Implementation of IXmlStorage
        event Action IXmlStorage.XmlChanged {
            add { ConfigurationChanged += value; }
            remove { ConfigurationChanged -= value; }
        }

        void IXmlStorage.SaveXml(XDocument xml)
        {
            XDocument config = GetConfigXml();
            XElement xContainer = config.XPathSelectElement("/configuration/xml-resource-storage");
            xContainer.RemoveNodes();
            xContainer.Add(xml.Root);
            SaveConfigXml(config);
        }

        XDocument IXmlStorage.LoadXml()
        {
            XDocument config = GetConfigXml();
            XElement xContainer = config.XPathSelectElement("/configuration/xml-resource-storage");

            XDocument result = new XDocument();
            if (xContainer != null) {
                result.Add(xContainer);
            }
            return result;
        }
        #endregion
    }
}
