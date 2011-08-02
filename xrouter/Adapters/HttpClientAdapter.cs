using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Gateway;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Net;
using XRouter.Common;
using ObjectConfigurator;

namespace XRouter.Adapters
{
    /// <summary>
    /// Adapter poskytuje jednoduchy HTTP protocol sender, ktery umoznuje realizovat clienta serveru. 
    /// HTTP content muze byt libovolny XML dokument (typicky SOAP, tj. adapter umoznuje realizovat simple 
    /// RPC-style web service client). 
    /// </summary>
    [AdapterPlugin("HttpClientAdapter", "dodat description")]
    class HttpClientAdapter : Adapter
    {
        [ConfigurationItem("URI", "A URI prefix string is composed of a scheme (http), a host, an optional port, and an optional path. An example of a complete prefix string is 'http://www.contoso.com:8080/customerData/'.", "http://localhost:8080/")]
        public string Uri { set; get; }
       
        [ConfigurationItem("ContentType", "", "text/xml; charset=utf-8")]
        public string ContentType { set; get; }
 
        [ConfigurationItem("SOAPAction", "", "")]
        public string SOAPAction { set; get; }
     
        [ConfigurationItem("TimeOut", "Time-out value in seconds.", 60)]
        public int TimeOut { set; get; }

        protected override void Run()
        {
            
        }

        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Uri);
            request.Timeout = this.TimeOut * 1000;

            // konfigurace requestu SOAP nad HTTP
            request.Method = "Post";
            request.ContentType = this.ContentType;
            if (this.SOAPAction != null)
            {
                request.Headers.Add("SOAPAction", this.SOAPAction);
            }
                                              
            // nastaveni kodovani
            Encoding encoding = Encoding.UTF8;
            if (message.Declaration != null)
            {
                string xmlEncoding = message.Declaration.Encoding;
                if (!string.IsNullOrEmpty(xmlEncoding))
                {
                    encoding = Encoding.GetEncoding(xmlEncoding);
                }
            }

            string strRequest = message.ToString();

            // odeslani zpravy serveru        
            byte[] bufferRequest = encoding.GetBytes(strRequest);
            request.ContentLength = bufferRequest.Length;

            Stream streamRequest = request.GetRequestStream();
            streamRequest.Write(bufferRequest, 0, bufferRequest.Length);
            streamRequest.Close();

            // ziskani odpovedi
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            XDocument xResponse = XDocument.Load(response.GetResponseStream(), LoadOptions.SetLineInfo);            

            return xResponse;
        }
    }
}
