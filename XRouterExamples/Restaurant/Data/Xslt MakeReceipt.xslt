<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
  <xsl:output method="text" encoding="windows-1250" />
  <xsl:template match="/">
    
    Restaurace MFF                  Úèet pro
    Malá Strana, Praha 1            stùl è. <xsl:value-of select="//Table" />
    
    Vydáno: <xsl:value-of select="//Date" />
    ------------------------------------------
    
    <xsl:for-each select="//Item"><xsl:value-of select="./Name" /> (<xsl:value-of select="./Quantity" />X) .............. <xsl:value-of select="./TotalPrice" />,00 Kè
    </xsl:for-each>
    ------------------------------------------

    Celkem (vè. DPH): .............. <xsl:value-of select="sum(//TotalPrice)" />,00 Kè

    Dìkujeme za Vaši návštìvu!

  </xsl:template>
</xsl:stylesheet>