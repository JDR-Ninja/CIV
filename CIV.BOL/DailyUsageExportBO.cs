using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CIV.BOL
{
    public class DailyUsageExportBO : DailyUsageBO
    {
        public string Username;

        public DailyUsageExportBO()
        {

        }

        public DailyUsageExportBO(DailyUsageBO usage, string username)
        {
            Username = username;
            Day = usage.Day;
            Month = usage.Month;
            Download = usage.Download;
            Upload = usage.Upload;
            Total = usage.Total;
            Period = usage.Period;
            /*if (String.IsNullOrEmpty(usage.Period))
                Period = usage.Period;
            else
                Period = usage.Period;*/
        }
    }
}
