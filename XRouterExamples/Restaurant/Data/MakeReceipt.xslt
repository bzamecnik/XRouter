<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
  <xsl:output method="text" encoding="windows-1250" />
  <xsl:template match="/">
    
    Restaurace MFF                  Účet pro
    Malá Strana, Praha 1            stůl č. <xsl:value-of select="//Table" />
    
    Vydáno: <xsl:value-of select="//Date" />
    ------------------------------------------
    
    <xsl:for-each select="//Item"><xsl:value-of select="./Name" /> (<xsl:value-of select="./Quantity" />X) .............. <xsl:value-of select="./TotalPrice" />,00 Kč
    </xsl:for-each>
    ------------------------------------------

    Celkem (vč. DPH): .............. <xsl:value-of select="sum(//TotalPrice)" />,00 Kč

    Děkujeme za Vaši návštěvu!

  </xsl:template>
</xsl:stylesheet>
