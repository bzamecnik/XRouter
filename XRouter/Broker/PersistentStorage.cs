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
                <adapter name='directoryAdapter' type='directoryAdapter'>
                    <objectConfig>
                    </objectConfig>
                </adapter>
            </adapters>
        </gateway>
        <processor name='processor1' concurrent-threads='4'>
        </processor>
    </components>
    <dispatcher nonrunning-processor-response-timeout='60'>
    </dispatcher>
    <adapter-types>
        <adapter-type name='directoryAdapter' clr-type='XRouter.Adapters.DirectoryAdapter,..\..\..\..\xrouter\Adapters\bin\debug\XRouter.Adapters.dll' />
    </adapter-types>
    <messageflows current='bcf9e0dd-818d-492c-84b6-4c27ca668221'>
        <messageflow guid='bcf9e0dd-818d-492c-84b6-4c27ca668221' name='abc' version='1'>
        </messageflow>
    </messageflows>
    <xml-resource-storage>

        <item name='schematron_sample1_detection'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'> 
             <pattern>
              <rule context='/'>
               <assert test='message'/>
              </rule>
              <rule context='/message/a|/message/b|/message/c'>   
               <assert test='true()'/>
              </rule>

              <rule context='item'>
                <assert test='false()'/>
              </rule>
              <!--
                <rule context='*'>
                  <assert test='false()'/>
                </rule>
              -->
             </pattern>
            </schema>
        </item>

         <item name='schematron_sample1_contains_A'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'> 
             <pattern>
              <rule context='/message'>
               <assert test='a'/>
              </rule>
             </pattern>
            </schema>
         </item>

        <item name='schematron_sample1_contains_B'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'> 
             <pattern>
              <rule context='/message'>
               <assert test='b'/>  
              </rule>
             </pattern>
            </schema>
         </item>

        <item name='schematron_sample1_contains_C'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'>
             <pattern>
              <rule context='/message'>
               <assert test='c'/>
              </rule>
             </pattern>
            </schema>
         </item>

        <item name='schematron_sample2_detection'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron'>
             <pattern>
              <rule context='/'>
               <assert test='message'/>
              </rule>
              <rule context='/message/item'>
               <assert test='true()'/>
              </rule>

              <!--
                <rule context='*'>
                  <assert test='false()'/>
                </rule>
              -->
             </pattern>
            </schema> 
        </item>

        <item name='schematron_sample2_targetA'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'> 
             <pattern>
              <rule context='/message'>
                <assert test='10>sum(item/@price) and sum(item/@price)>=0' />
              </rule> 
             </pattern>
            </schema>
         </item>

        <item name='schematron_sample2_targetB'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'> 
             <pattern>
              <rule context='/message'>
                <assert test='20>sum(item/@price) and sum(item/@price)>=10' />
              </rule> 
             </pattern>
            </schema>
         </item>

        <item name='schematron_sample2_targetC'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'> 
             <pattern>
              <rule context='/message'>
                <assert test='30>sum(item/@price) and sum(item/@price)>=20' />
              </rule> 
             </pattern>
            </schema>
         </item>

        <item name='schematron_sample3_detection'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'>
             <ns prefix='S' uri='http://www.w3.org/2003/05/soap-envelope'/> 
             <pattern>
              <rule context='/'>
               <assert test='S:Envelope'/>
              </rule>
             </pattern>  
            </schema>
         </item>

        <item name='schematron_sample3_targetA'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'>
             <ns prefix='S' uri='http://www.w3.org/2003/05/soap-envelope'/> 
             <ns prefix='wsa' uri='http://www.w3.org/2005/08/addressing'/> 
             <pattern>
               <rule context='/S:Envelope/S:Header/wsa:To'>
                  <assert test="".='OutA'""/>
               </rule>
             </pattern>  
            </schema>
         </item>

        <item name='schematron_sample3_targetB'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'>
             <ns prefix='S' uri='http://www.w3.org/2003/05/soap-envelope'/> 
             <ns prefix='wsa' uri='http://www.w3.org/2005/08/addressing'/> 
             <pattern>
               <rule context='/S:Envelope/S:Header/wsa:To'>
                  <assert test="".='OutB'""/>
               </rule>
             </pattern>  
            </schema>
         </item>

        <item name='schematron_sample3_targetC'>
            <schema xmlns='http://purl.oclc.org/dsdl/schematron' queryBinding='xpath'>
             <ns prefix='S' uri='http://www.w3.org/2003/05/soap-envelope'/> 
             <ns prefix='wsa' uri='http://www.w3.org/2005/08/addressing'/> 
             <pattern>
               <rule context='/S:Envelope/S:Header/wsa:To'>
                  <assert test="".='OutC'""/>
               </rule>
             </pattern>  
            </schema>
         </item>

        <item name='xslt_sample3'>
            <xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:S='http://www.w3.org/2003/05/soap-envelope'>
             <xsl:template match='node()|@*'>
              <xsl:copy>
               <xsl:apply-templates select='node()|@*'/>
              </xsl:copy>
             </xsl:template>
             <xsl:template match='S:Header'/>
            </xsl:stylesheet>
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

        public Token[] GetTokens(int pageSize, int pageNumber)
        {
            int tokensToSkip = (pageNumber - 1) * pageSize;
            var result = InternalTokens.Skip(tokensToSkip).Take(pageNumber).ToArray();
            return result;
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
