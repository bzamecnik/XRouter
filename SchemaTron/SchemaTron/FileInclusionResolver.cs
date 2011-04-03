using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SchemaTron
{
    /// <summary>
    /// A default implementation of resolving included elements.
    /// </summary>
    /// <remarks>
    /// Internally it caches the loaded documents.
    /// </remarks>
    internal sealed class FileInclusionResolver : IInclusionResolver
    {
        /// <summary>
        /// A cache of previously loaded documents.
        /// </summary>
        private Dictionary<String, XDocument> loadedDocs = new Dictionary<String, XDocument>();

        /// <summary>
        /// Loads an external XML document from a file specified in the
        /// <c>href</c> parameter.
        /// </summary>
        /// <param name="href">Absolute path to the external XML document.</param>
        /// <returns>The loaded external XML document.</returns>    
        public XDocument Resolve(String href)
        {
            // each external XML dokument is loaded only once
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
