using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Adapters;
using Xunit;

namespace XRouter.Adapters.Test
{
    // NOTE: The following test are not fully automatic as they expect an SMTP
    // server running at a specified host and it has to be verified by hand
    // that a correct email arrived.
    class EMailClientAdapterTest
    {
        [Fact]
        public void SendMailWithNullMessage()
        {
            EMailClientAdapter adapter = PrepareEMailAdapter();
            Assert.DoesNotThrow(() => adapter.SendMessage("", null, null));
        }

        [Fact]
        public void SendMailWithXmlMessageAndNullMetadata()
        {
            EMailClientAdapter adapter = PrepareEMailAdapter();
            XDocument message = PrepareXmlMessage();
            Assert.Throws<ArgumentNullException>(() =>
                adapter.SendMessage("", message, null));
        }

        [Fact]
        public void SendMailWithXmlMessageAndSomeMetadata()
        {
            EMailClientAdapter adapter = PrepareEMailAdapter();
            XDocument message = PrepareXmlMessage();
            XDocument metadata = XDocument.Parse(@"<attachmentName>CallGetReceipt.xslt</attachmentName>");
            Assert.DoesNotThrow(() => adapter.SendMessage("", message, metadata));
        }

        private static EMailClientAdapter PrepareEMailAdapter()
        {
            EMailClientAdapter adapter = new EMailClientAdapter()
            {
                SmtpHost = "192.168.10.1",
                SmtpPort = 25,
                From = "xrouter-service@xrouter.dyndns.info",
                FromDisplayName = "XRouter",
                To = new[] { "xrouter-admin@xrouter.dyndns.info" }.ToList(),
                Subject = "EMailClientAdapter test",
                Body = "This is the body of the EMailClientAdapter test."
            };
            return adapter;
        }

        private static XDocument PrepareXmlMessage()
        {
            XDocument message = XDocument.Parse(
@"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:msxsl='urn:schemas-microsoft-com:xslt' version='1.0' exclude-result-prefixes='msxsl'>
  <xsl:output method='xml' encoding='utf-8' />
  <xsl:template match='/'>
    <s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>
      <s:Body>
        <GetReceipt xmlns='http://tempuri.org/'>
          <payment xmlns:i='http://schemas.datacontract.org/2004/07/XRouterWS'>
            <i:table><xsl:value-of select='//table' /></i:table>
          </payment>
        </GetReceipt>
      </s:Body>
    </s:Envelope>
  </xsl:template>
</xsl:stylesheet>
");
            return message;
        }
    }
}
