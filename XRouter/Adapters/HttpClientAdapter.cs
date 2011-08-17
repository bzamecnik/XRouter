using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Gateway;

namespace XRouter.Adapters
{
    /// <summary>
    /// HTTP client adapter provides a simple RPC-style client which can send
    /// messages to remote web services synchronously. The content can be
    /// arbitrary XML document (typically SOAP).
    /// </summary>
    /// <remarks>
    /// The client sends the XML content to a service specified by its target
    /// URI using a specified SOAP action. The request can be terminated after
    /// given timeout. After sending the request a response from the web
    /// service is returend.
    /// </remarks>
    [AdapterPlugin("HTTP client adapter", "Provides a HTTP client which can send messages to remote web services.")]
    public class HttpClientAdapter : Adapter
    {
        [ConfigurationItem("Target URI", "URI of the target web service. Example: 'http://www.example.com:8080/path/'.", "http://localhost:8080/")]
        public string Uri { set; get; }

        [ConfigurationItem("Content type", "", "text/xml; charset=utf-8")]
        public string ContentType { set; get; }

        [ConfigurationItem("SOAP action", "", "")]
        public string SOAPAction { set; get; }

        [ConfigurationItem("Timeout", "Timeout in seconds.", 60)]
        public int TimeOut { set; get; }

        protected override void Run()
        {
            // NOTE: the adapter does not listen to receive any messagess
            // See the HttpServiceAdapter for HTTP server functionality.
        }

        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Uri);
            request.Timeout = this.TimeOut * 1000;

            // configuring a SOAP request over HTTP
            request.Method = "Post";
            request.ContentType = this.ContentType;
            if (this.SOAPAction != null)
            {
                request.Headers.Add("SOAPAction", this.SOAPAction);
            }

            // encoding
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

            // send the message to the server
            byte[] bufferRequest = encoding.GetBytes(strRequest);
            request.ContentLength = bufferRequest.Length;

            Stream streamRequest = request.GetRequestStream();
            streamRequest.Write(bufferRequest, 0, bufferRequest.Length);
            streamRequest.Close();

            // wait for the response - synchronously
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            XDocument xResponse = XDocument.Load(response.GetResponseStream(), LoadOptions.SetLineInfo);

            return xResponse;
        }
    }
}
