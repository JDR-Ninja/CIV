using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "internetAccounts", IsNullable = true)]
    public class InternetAccounts
    {
        [XmlElementAttribute("WiredInternetAccountUsage")]
        public List<WiredInternetAccountUsage> WiredInternetAccountUsage { get; set; }

        public InternetAccounts()
        {
            WiredInternetAccountUsage = new List<WiredInternetAccountUsage>();
        }
    }
}
