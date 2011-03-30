using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchemaTron
{
    /// <summary>
    /// Defaultni implementace dodani included elementu. 
    /// </summary>
    internal sealed class InclusionsFileResolver : InclusionsResolver
    {    
        private Dictionary<String, XDocument> loadedDocs = new Dictionary<String, XDocument>();
        
        /// <summary>
        /// Nacte externi XML dokument ze souboru, jehoz umisteni je dano atributem href.
        /// </summary>
        /// <param name="href">Absolutni cesta k externimu XML dokumentu.</param>
        /// <returns></returns>    
        public XDocument Resolve(String href)
        {
            // kazdy externi XML dokument je nacten prave jednou
            XDocument doc;
            if (!this.loadedDocs.TryGetValue(href, out doc))          
            {
                doc = XDocument.Load(href, LoadOptions.SetLineInfo);
                this.loadedDocs.Add(href, doc);               
            }

            return doc;
        }
    }
}
