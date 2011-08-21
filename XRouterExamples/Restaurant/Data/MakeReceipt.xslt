<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="text" encoding="windows-1250"/>
  
  <xsl:template match="/">
    
    Restaurace MFF                  Účet pro
    Malá Strana, Praha 1            stůl č. <xsl:value-of select="//table"/>
    
    Vydáno: <xsl:value-of select="//date"/>
    ------------------------------------------
    
    <xsl:for-each select="//item">
      <xsl:value-of select="./@name"/> (<xsl:value-of select="./quantity"/>X) .............. <xsl:value-of select="./totalPrice"/>,00 Kč
    </xsl:for-each>
    ------------------------------------------

    Celkem (vč. DPH): .............. <xsl:value-of select="sum(//totalPrice)"/>,00 Kč

    Děkujeme za Vaši návštěvu!

  </xsl:template>
</xsl:stylesheet>
