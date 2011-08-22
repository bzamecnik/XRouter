<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" encoding="utf-8"/>
  <xsl:template match="/">
    <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
      <s:Body>
        <SaveOrder xmlns="http://tempuri.org/">
          <order xmlns:i="http://schemas.datacontract.org/2004/07/XRouterWS">
            <i:item><xsl:value-of select="//item"/></i:item>
            <i:table><xsl:value-of select="//table"/></i:table>
          </order>
        </SaveOrder>
      </s:Body>
    </s:Envelope>
  </xsl:template>
</xsl:stylesheet>
