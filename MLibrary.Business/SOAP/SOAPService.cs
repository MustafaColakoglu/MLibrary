using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace MLibrary.Business.SOAP
{
    class SOAPService : ISOAPService
    {
        public HttpWebRequest CreateWebRequest(string type,string url)
        {
            String username = "webrfc";
            String password = "a1234567";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Headers.Add("Authorization", "Basic " + encoded);
            webRequest.Accept = "text/xml";
            webRequest.Method = type;
            return webRequest;
        }

        public string SOAPManual(string stringXML,string url,string type)
        {
            

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(stringXML);
            HttpWebRequest webRequest = CreateWebRequest(type,url);

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            string result;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    result = rd.ReadToEnd();
                }
            }
            return result;
        }
    }
}
