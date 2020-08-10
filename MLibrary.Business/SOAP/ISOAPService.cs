using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MLibrary.Business.SOAP
{
    public interface ISOAPService
    {
        string SOAPManual(string stringXML, string url, string type);



    }
}
