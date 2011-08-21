<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" encoding="utf-8" />
  <xsl:template match="/">
    <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
      <s:Body>
        <GetReceipt xmlns="http://tempuri.org/">
          <payment xmlns:i="http://schemas.datacontract.org/2004/07/XRouterWS">
            <i:table><xsl:value-of select="//table" /></i:table>
          </payment>
        </GetReceipt>
      </s:Body>
    </s:Envelope>
  </xsl:template>
</xsl:stylesheet>