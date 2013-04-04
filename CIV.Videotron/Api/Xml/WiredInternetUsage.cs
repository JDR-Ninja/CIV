using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "WiredInternetUsage", IsNullable = true)]
    public class WiredInternetUsage
    {
        [XmlElementAttribute("messages")]
        public Messages Messages { get; set; }

        [XmlElementAttribute("apiVersion")]
        public string ApiVersion { get; set; }

        [XmlElementAttribute("periodStartDate")]
        public DateTime PeriodStartDate { get; set; }

        [XmlElementAttribute("periodEndDate")]
        public DateTime PeriodEndDate { get; set; }

        [XmlElementAttribute("daysFromStart")]
        public int DaysFromStart { get; set; }

        [XmlElementAttribute("daysToEnd")]
        public int DaysToEnd { get; set; }

        [XmlElementAttribute("internetAccounts")]
        public InternetAccounts InternetAccounts { get; set; }
    }
}
