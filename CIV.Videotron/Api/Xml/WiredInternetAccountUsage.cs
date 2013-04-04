using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "WiredInternetAccountUsage", IsNullable = true)]
    public class WiredInternetAccountUsage
    {
        [XmlElementAttribute("internetAccountNo")]
        public string InternetAccountNo { get; set; }

        [XmlElementAttribute("messages")]
        public Messages Messages { get; set; }

        [XmlElementAttribute("maxDownloadBytes")]
        public long MaxDownloadBytes { get; set; }

        [XmlElementAttribute("maxUploadBytes")]
        public long MaxUploadBytes { get; set; }

        [XmlElementAttribute("maxCombinedBytes")]
        public long MaxCombinedBytes { get; set; }

        //[XmlElementAttribute("")]
        //public string PackageName { get; set; }

        //[XmlElementAttribute("")]
        //public List<string> PackageDetails { get; set; }

        //[XmlElementAttribute("")]
        //public DateTime PackageTimestamp { get; set; }

        [XmlElementAttribute("downloadedBytes")]
        public long DownloadedBytes { get; set; }

        [XmlElementAttribute("uploadedBytes")]
        public long UploadedBytes { get; set; }

        [XmlElementAttribute("downloadedPercent")]
        public double DownloadedPercent { get; set; }

        [XmlElementAttribute("uploadedPercent")]
        public double UploadedPercent { get; set; }

        [XmlElementAttribute("combinedPercent")]
        public double CombinedPercent { get; set; }

        [XmlElementAttribute("packageCode")]
        public double PackageCode { get; set; }

        [XmlIgnore]
        public DateTime UsageTimestamp { get; set; }

        [XmlElementAttribute("usageTimestamp")]
        public string UsageTimestampRaw
        {
            get
            {
                return UsageTimestamp.ToString(XmlConstant.XML_DATE_FORMAT);
            }

            set
            {
                Match match = Regex.Match(value, @"^(?<date>\d{4}-\d{2}-\d{2})T(?<hour>\d{2}:\d{2})", RegexOptions.IgnoreCase);
                if (match.Success)
                {

                    UsageTimestamp = DateTime.ParseExact(String.Format("{0} {1}", match.Groups["date"].Value, match.Groups["hour"].Value),
                                                     "yyyy-MM-dd HH:mm",
                                                     CultureInfo.InvariantCulture);
                }
                else
                {
                    match = Regex.Match(value, @"^(?<date>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase);
                    if (match.Success)
                        UsageTimestamp = DateTime.ParseExact(match.Groups["date"].Value,
                                                     "yyyy-MM-dd",
                                                     CultureInfo.InvariantCulture);
                }
            }
        }

        [XmlElementAttribute("dailyUsage")]
        public DailyUsage DailyUsage { get; set; }
    }
}
