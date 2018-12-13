using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace NascoWebAPI.Helper
{
    public static class XMLHelper
    {
        #region XML
        public static XmlDocument SerializeToXmlDocument(object obj)
        {
            XmlDocument doc = new XmlDocument();
            XPathNavigator nav = doc.CreateNavigator();
            using (XmlWriter w = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(obj.GetType());
                ser.Serialize(w, obj);
            }
            return doc;
        }
        public static T Deserialize<T>(string content) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                var obj = (T)serializer.Deserialize(reader);
                return obj;
            }
        }
        public static async Task<XElement> PostEncodedContent(string requestUri, object content)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = "POST";
            XmlDocument soapEnvelopeXml = new XmlDocument();
            XmlDocument xmlBody = new XmlDocument();
            if (content is string)
            {
                xmlBody.LoadXml(content + "");
            }
            else
            {
                xmlBody = SerializeToXmlDocument(content);
            }

            soapEnvelopeXml.LoadXml(string.Format(@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body>
            {0}</soap:Body></soap:Envelope>", xmlBody.OuterXml));
            using (Stream stream = await request.GetRequestStreamAsync())
            {
                soapEnvelopeXml.Save(stream);
            }
            string soapResult = "";
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
            }
            catch (WebException webex)
            {
                using (WebResponse response = webex.Response)
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
            }
            XElement xElement = XElement.Parse(soapResult);
            XNamespace xNamespace = xmlBody.DocumentElement.NamespaceURI;
            var result = xElement.Descendants(xNamespace + xmlBody.DocumentElement.Name + "Result").First();
            return result;
        }
        public static async Task<T> PostEncodedContent<T>(string requestUri, object content) where T : class
        {
            XElement result = await PostEncodedContent(requestUri, content);
            if (typeof(T) == typeof(String))
            {
                return result.Value as T;
            }
            return Deserialize<T>(result.Elements().First().ToXmlNode().OuterXml);
        }
        #endregion
        #region Extention
        public static XElement ToXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        public static XmlNode ToXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        #endregion
    }
}
