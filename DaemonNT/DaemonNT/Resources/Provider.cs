using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace DaemonNT.Resources
{
    /// <summary>
    /// Poskytuje pristup k embedded resources.
    /// </summary>
    internal static class Provider
    {
        public static XDocument LoadConfigSchema()
        {            
            Assembly currAssembly = Assembly.GetExecutingAssembly();
            Stream stream = currAssembly.GetManifestResourceStream("DaemonNT.Resources.config_schema.xml");
            XDocument xSchema = XDocument.Load(stream, LoadOptions.SetLineInfo);
            return xSchema;
        }
    }
}
