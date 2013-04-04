using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;

namespace Videotron.Wired
{
    public class WiredDailyUsage
    {
        public DateTime Day;

        public string Month
        {
            get
            {
                return Day.Year.ToString() + Day.Month.ToString().PadLeft(2, '0');
            }
        }
        
        public double Download;
        public double Upload;
        public double Total
        {
            get
            {
                return Upload + Download;
            }
        }

        public Period Period;

        public WiredDailyUsage()
        {

        }

        public WiredDailyUsage(DateTime day, double upload, double download)
        {
            Day = day;
            Upload = upload;
            Download = download;
        }
    }
}
