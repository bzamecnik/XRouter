using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace SchemaTron.Resources
{
    internal static class Provider
    {
        private static XDocument schemaPhaseA = null;

        public static XDocument SchemaPhaseA
        {
            get
            {
                lock (typeof(Provider))
                {
                    if (schemaPhaseA == null)
                    {
                        schemaPhaseA = LoadXmlDocument("SchemaTron.Resources.schema_phaseA.xml");
                    }
                }

                return schemaPhaseA;
            }
        }

        private static XDocument schemaPhaseB = null;

        public static XDocument SchemaPhaseB
        {
            get
            {
                lock (typeof(Provider))
                {
                    if (schemaPhaseB == null)
                    {
                        schemaPhaseB = LoadXmlDocument("SchemaTron.Resources.schema_phaseB.xml");
                    }
                }

                return schemaPhaseB;
            }
        }

        private static XDocument schemaPhaseC = null;

        public static XDocument SchemaPhaseC
        {
            get
            {
                lock (typeof(Provider))
                {
                    if (schemaPhaseC == null)
                    {
                        schemaPhaseC = LoadXmlDocument("SchemaTron.Resources.schema_phaseC.xml");
                    }
                }

                return schemaPhaseC;
            }
        }

        private static XDocument LoadXmlDocument(String name)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Stream stream = currentAssembly.GetManifestResourceStream(name);
            XDocument xDoc = XDocument.Load(stream);
            return xDoc;
        }
    }
}
