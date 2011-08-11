using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Data;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Common.Persistence
{
    class MemoryDataAccess : IDataAccess_New
    {
        private object storageLock = new object();

        private string configurationXml;

        private Dictionary<Guid, string> tokens = new Dictionary<Guid, string>();

        public void Initialize(string connectionString)
        {
            #region In memory configurationXml
            configurationXml = @"
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
";
            #endregion
        }

        public void SaveConfiguration(string configXml)
        {
            lock (storageLock) {
                configurationXml = configXml;
            }
        }

        public string LoadConfiguration()
        {
            lock (storageLock) {
                return configurationXml;
            }
        }

        public void SaveToken(Guid tokenGuid, string tokenXml)
        {
            lock (storageLock) {
                if (tokens.ContainsKey(tokenGuid)) {
                    tokens[tokenGuid] = tokenXml;
                } else {
                    tokens.Add(tokenGuid, tokenXml);
                }
            }
        }

        public string LoadToken(Guid tokenGuid)
        {
            lock (storageLock) {
                if (tokens.ContainsKey(tokenGuid)) {
                    return tokens[tokenGuid];
                } else {
                    return null;
                }
            }
        }

        public IEnumerable<string> LoadTokens(int pageSize, int pageNumber)
        {
            lock (storageLock) {
                int tokensToSkip = (pageNumber - 1) * pageSize;
                return tokens.Values.Skip(tokensToSkip).Take(pageNumber);
            }
        }

        public IEnumerable<string> LoadMatchingTokens(string xpath)
        {
            var result = tokens.Values.Where(tokenXml => {
                XDocument xToken = XDocument.Parse(tokenXml);
                object matching = xToken.XPathEvaluate(xpath);
                return matching != null;
            });
            return result;
        }
    }
}
