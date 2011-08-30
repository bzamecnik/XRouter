<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
  <xsl:output method="text" encoding="windows-1250" />
  <xsl:template match="/">
    
    Restaurace MFF                  ��et pro
    Mal� Strana, Praha 1            st�l �. <xsl:value-of select="//Table" />
    
    Vyd�no: <xsl:value-of select="//Date" />
    ------------------------------------------
    
    <xsl:for-each select="//Item"><xsl:value-of select="./Name" /> (<xsl:value-of select="./Quantity" />X) .............. <xsl:value-of select="./TotalPrice" />,00 K�
    </xsl:for-each>
    ------------------------------------------

    Celkem (v�. DPH): .............. <xsl:value-of select="sum(//TotalPrice)" />,00 K�

    D�kujeme za Va�i n�v�t�vu!

  </xsl:template>
</xsl:stylesheet>