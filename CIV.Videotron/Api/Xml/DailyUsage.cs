using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "dailyUsage", IsNullable = true)]
    public class DailyUsage
    {
        [XmlElementAttribute("WiredInternetDailyUsage")]
        public List<WiredInternetDailyUsage> WiredInternetDailyUsage { get; set; }

        public DailyUsage()
        {
            WiredInternetDailyUsage = new List<WiredInternetDailyUsage>();
        }
    }
}
