using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "WiredInternetDailyUsage", IsNullable = true)]
    public class WiredInternetDailyUsage
    {
        [XmlElementAttribute("date")]
        public DateTime Date { get; set; }

        [XmlElementAttribute("downloadedBytes")]
        public long DownloadedBytes { get; set; }

        [XmlElementAttribute("uploadedBytes")]
        public long UploadedBytes { get; set; }
    }
}
