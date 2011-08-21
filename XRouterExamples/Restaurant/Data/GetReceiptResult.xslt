﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" encoding="windows-1250"/>

  <xsl:template match="/">
    <xsl:apply-templates select="//*[local-name(.)='GetReceiptResult']"/>
  </xsl:template>
  
    <xsl:template match="*" >
      <xsl:element name="{local-name(.)}">
        <xsl:apply-templates/>
      </xsl:element>
    </xsl:template>

</xsl:stylesheet>
