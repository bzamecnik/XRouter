<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" encoding="windows-1250" />
  <xsl:template match="/">
    <xsl:apply-templates select="//*[local-name(.)='GetReceiptResult']" />
  </xsl:template>
  <xsl:template match="*">
    <xsl:element name="{local-name(.)}">
      <xsl:apply-templates />
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>