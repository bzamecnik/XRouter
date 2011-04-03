using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Reflection;

namespace UnitTests.Resources
{
    internal static class Provider
    {
        public static XDocument LoadXmlDocument(string name)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Stream stream = currentAssembly.GetManifestResourceStream(String.Format("UnitTests.Resources.{0}", name));
            XDocument xDoc = XDocument.Load(stream, LoadOptions.SetLineInfo);
            return xDoc;
        }
    }
}
