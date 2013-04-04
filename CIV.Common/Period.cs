using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CIV.Common
{
    public class Period
    {
        private DateTime _start;

        public DateTime Start
        {
            get { return _start; }
            set { _start = value; }
        }

        private DateTime _end;

        public DateTime End
        {
            get { return _end; }
            set { _end = value; }
        }

        public Period()
        {

        }

        public Period(string text)
        {
            Start = DateTime.ParseExact(text.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
            End = DateTime.ParseExact(text.Substring(9, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return String.Format("{0}@{1}", Start.ToString("yyyyMMdd"), End.ToString("yyyyMMdd"));
        }
    }
}
