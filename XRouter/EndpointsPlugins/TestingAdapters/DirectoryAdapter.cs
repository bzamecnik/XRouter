using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Gateway;
using XRouter.Gateway.Implementation;

namespace XRouter.Adapters.TestingAdapters
{
	class DirectoryAdapter : IAdapter
	{
		public string Name { get; private set; }
		private XElement Configuration { get; set; }
		private IAdapterService Service { get; set; }

		bool isCanceled;

		#region AdapterConfiguration
        Dictionary<string, string> monitoredPaths = new Dictionary<string, string>();
		int timeOut;
		#endregion

		public DirectoryAdapter(XElement configuration, IAdapterService service)
		{
			this.Service = service;
			Configuration = configuration;
			Name = Configuration.Attribute(XName.Get("name")).Value;

			// Load configuration
			timeOut = Convert.ToInt32(Configuration.Attribute(XName.Get("timeOut")).Value);
			//! Nacteni jednotlivych cest k souborum ktere budem kontrolovat
		}

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
		{
			try
			{
				string fileName = null;
                string strData = message.ToString();

				FileStream fs = new FileStream(fileName, FileMode.Create);
				StreamWriter sw = new StreamWriter(fs);

				sw.Write(strData);
				sw.Close();
			}
			catch (Exception)
			{
				// zalogovani chyby
			}
            return null;
		}

		private void ReceiveMessage(XDocument xDoc, EndpointAddress sourceAddress)
		{
            Token token = new Token();
            token.AddMessage(xDoc);
            token.SourceAddress = sourceAddress;
            Service.ReceiveMessageAsync(token);
		}

		public void Start()
		{
            Dictionary<string, string> receivedFiles = new Dictionary<string, string>();
			isCanceled = false;

			while (!isCanceled)
			{
				foreach (var pair in monitoredPaths)
				{
					if (File.Exists(pair.Value))
					{
						receivedFiles.Add(pair.Key, pair.Value);
					}
				}
				foreach (var pair in receivedFiles)
				{
					XDocument xDoc = XDocument.Load(pair.Value);
                    ReceiveMessage(xDoc, new EndpointAddress(Name, pair.Key));
				}
				Thread.Sleep(timeOut);
			}
		}

		public void Stop()
		{
			isCanceled = true;
		}
	}
}

