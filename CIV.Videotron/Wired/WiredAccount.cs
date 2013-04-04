using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Wired
{
    public class WiredAccount
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int DaysElapsed { get; set; }
        public int DaysRemaining { get; set; }
        public string Username { get; set; }

        public double MaxDownloadBytes { get; set; }
        public double MaxUploadBytes { get; set; }
        public double MaxCombinedBytes { get; set; }
        public double DownloadedBytes { get; set; }
        public double UploadedBytes { get; set; }
        public double DownloadedPercent { get; set; } // Si le quota est par download
        public double UploadedPercent { get; set; } // Si le quota est par upload
        public double CombinedPercent { get; set; } // Si le quota est par combined
        //public DateTime UsageTimestamp { get; set; }
        
        //public string PackageName { get; set; }
        public List<WiredDailyUsage> DailyUsage { get; set; }
        public List<WiredMessage> Messages { get; set; }
        //public List<string> PackageDetails { get; set; } // TODO à voir...
        //public DateTime PackageTimestamp { get; set; }
        
        //public WiredInternetDailyUsage DailyUsage { get; set; }

        public WiredAccount()
        {
            DailyUsage = new List<WiredDailyUsage>();
            Messages = new List<WiredMessage>();
        }
    }
}
