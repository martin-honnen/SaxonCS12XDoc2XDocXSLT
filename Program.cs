using Saxon.Api;
using System.Xml.Linq;

XDocument doc = XDocument.Parse(@"<root>
  <item>
    <name>Item 1</name>
    <categories>
      <cat>a</cat>
      <cat>b</cat>
    </categories>
  </item>
  <item>
    <name>Item 2</name>
    <categories>
      <cat>a</cat>
      <cat>c</cat>
    </categories>
  </item>
  <item>
    <name>Item 3</name>
    <categories>
      <cat>b</cat>
      <cat>c</cat>
    </categories>
  </item>
</root>");


Processor processor = new(true);

DocumentBuilder docBuilder = processor.NewDocumentBuilder();

XdmNode xdmDoc = docBuilder.Wrap(doc);

var xsltSource = @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema' version='3.0' exclude-result-prefixes='#all'>

  <xsl:output indent='yes'/>

  <xsl:mode on-no-match='shallow-copy'/>

  <xsl:template match='root'>
    <xsl:copy>
     <xsl:for-each-group select='item' group-by='categories/cat'>
        <category name='{current-grouping-key()}'>
          <xsl:apply-templates select='current-group()'/>
        </category>
     </xsl:for-each-group>
   </xsl:copy>
  </xsl:template>

  <xsl:template match='item/categories'/>

</xsl:stylesheet>
";

var xsltCompiler = processor.NewXsltCompiler();

var xsltExecutable = xsltCompiler.Compile(new StringReader(xsltSource));

var xslt30Transformer = xsltExecutable.Load30();

var result = new LinqDestination();

xslt30Transformer.ApplyTemplates(xdmDoc, result);

Console.WriteLine(result.XDocument);