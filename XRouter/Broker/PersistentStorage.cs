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
          <MessageFlowConfiguration xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" z:Id=""1"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/XRouter.Common.MessageFlowConfig"">
            <Guid>27063bda-5f37-4efc-841c-3c62ee31c4ba</Guid>
            <Name z:Id=""2"">messageflow1</Name>
            <Nodes z:Id=""3"" z:Size=""7"">
              <NodeConfiguration z:Id=""4"" i:type=""CbrNodeConfiguration"">
                <Name z:Id=""5"">switchA</Name>
                <Branches xmlns:d4p1=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"" z:Id=""6"" z:Size=""1"">
                  <d4p1:KeyValueOfXrmUriNodeConfigurationL2ieEqSk>
                    <d4p1:Key xmlns:d6p1=""http://schemas.datacontract.org/2004/07/XRouter.Common.Xrm"" z:Id=""7"">
                      <d6p1:XPath z:Id=""8"">//item[@name='containsA']</d6p1:XPath>
                    </d4p1:Key>
                    <d4p1:Value z:Id=""9"" i:type=""ActionNodeConfiguration"">
                      <Name z:Id=""10"">sendToA</Name>
                      <Actions z:Id=""11"" z:Size=""1"">
                        <ActionConfiguration z:Id=""12"">
                          <PluginConfiguration xmlns:d9p1=""http://schemas.datacontract.org/2004/07/XRouter.Common"" z:Id=""13"">
                            <d9p1:XmlContent z:Id=""14"">&lt;target&gt;A&lt;/target&gt;</d9p1:XmlContent>
                          </PluginConfiguration>
                          <PluginTypeFullName z:Id=""15"">XRouter.Processor.BuiltInActions.SendMessageAction</PluginTypeFullName>
                        </ActionConfiguration>
                      </Actions>
                      <NextNode z:Id=""16"" i:type=""CbrNodeConfiguration"">
                        <Name z:Id=""17"">switchB</Name>
                        <Branches z:Id=""18"" z:Size=""1"">
                          <d4p1:KeyValueOfXrmUriNodeConfigurationL2ieEqSk>
                            <d4p1:Key xmlns:d10p1=""http://schemas.datacontract.org/2004/07/XRouter.Common.Xrm"" z:Id=""19"">
                              <d10p1:XPath z:Id=""20"">//item[@name='containsB']</d10p1:XPath>
                            </d4p1:Key>
                            <d4p1:Value z:Id=""21"" i:type=""ActionNodeConfiguration"">
                              <Name z:Id=""22"">sendToB</Name>
                              <Actions z:Id=""23"" z:Size=""1"">
                                <ActionConfiguration z:Id=""24"">
                                  <PluginConfiguration xmlns:d13p1=""http://schemas.datacontract.org/2004/07/XRouter.Common"" z:Id=""25"">
                                    <d13p1:XmlContent z:Id=""26"">&lt;target&gt;B&lt;/target&gt;</d13p1:XmlContent>
                                  </PluginConfiguration>
                                  <PluginTypeFullName z:Ref=""15"" i:nil=""true"" />
                                </ActionConfiguration>
                              </Actions>
                              <NextNode z:Id=""27"" i:type=""CbrNodeConfiguration"">
                                <Name z:Id=""28"">switchC</Name>
                                <Branches z:Id=""29"" z:Size=""1"">
                                  <d4p1:KeyValueOfXrmUriNodeConfigurationL2ieEqSk>
                                    <d4p1:Key xmlns:d14p1=""http://schemas.datacontract.org/2004/07/XRouter.Common.Xrm"" z:Id=""30"">
                                      <d14p1:XPath z:Id=""31"">//item[@name='containsC']</d14p1:XPath>
                                    </d4p1:Key>
                                    <d4p1:Value z:Id=""32"" i:type=""ActionNodeConfiguration"">
                                      <Name z:Id=""33"">sendToC</Name>
                                      <Actions z:Id=""34"" z:Size=""1"">
                                        <ActionConfiguration z:Id=""35"">
                                          <PluginConfiguration xmlns:d17p1=""http://schemas.datacontract.org/2004/07/XRouter.Common"" z:Id=""36"">
                                            <d17p1:XmlContent z:Id=""37"">&lt;target&gt;C&lt;/target&gt;</d17p1:XmlContent>
                                          </PluginConfiguration>
                                          <PluginTypeFullName z:Ref=""15"" i:nil=""true"" />
                                        </ActionConfiguration>
                                      </Actions>
                                      <NextNode z:Id=""38"" i:type=""TerminatorNodeConfiguration"">
                                        <Name z:Id=""39"">term1</Name>
                                        <IsReturningOutput>false</IsReturningOutput>
                                        <ResultMessageSelection i:nil=""true"" />
                                      </NextNode>
                                    </d4p1:Value>
                                  </d4p1:KeyValueOfXrmUriNodeConfigurationL2ieEqSk>
                                </Branches>
                                <DefaultTarget z:Ref=""38"" i:nil=""true"" />
                                <TestedSelection z:Id=""40"">
                                  <SelectionPattern z:Id=""41"">/token/messages/message[@name='input']/*[1]</SelectionPattern>
                                </TestedSelection>
                              </NextNode>
                            </d4p1:Value>
                          </d4p1:KeyValueOfXrmUriNodeConfigurationL2ieEqSk>
                        </Branches>
                        <DefaultTarget z:Ref=""27"" i:nil=""true"" />
                        <TestedSelection z:Ref=""40"" i:nil=""true"" />
                      </NextNode>
                    </d4p1:Value>
                  </d4p1:KeyValueOfXrmUriNodeConfigurationL2ieEqSk>
                </Branches>
                <DefaultTarget z:Ref=""16"" i:nil=""true"" />
                <TestedSelection z:Ref=""40"" i:nil=""true"" />
              </NodeConfiguration>
              <NodeConfiguration z:Ref=""16"" i:nil=""true"" />
              <NodeConfiguration z:Ref=""27"" i:nil=""true"" />
              <NodeConfiguration z:Ref=""9"" i:nil=""true"" />
              <NodeConfiguration z:Ref=""21"" i:nil=""true"" />
              <NodeConfiguration z:Ref=""32"" i:nil=""true"" />
              <NodeConfiguration z:Ref=""38"" i:nil=""true"" />
            </Nodes>
            <RootNode z:Ref=""4"" i:nil=""true"" />
            <Version>1</Version>
          </MessageFlowConfiguration>
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
