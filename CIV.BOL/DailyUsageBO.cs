using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;

namespace CIV.BOL
{
    public class DailyUsageBO
    {
        public DateTime Day { get; set; }
        public string Month { get; set; }
        public double Download { get; set; }
        public double Upload { get; set; }
        public double Total { get; set; }
        public Period Period { get; set; } // 20100903-20101003
    }
}
