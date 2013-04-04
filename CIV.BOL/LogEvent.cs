using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.BOL
{
    public class LogEvent
    {
        public Guid Id;
        public DateTime Created;
        public string OsVersion;
        public bool Is64BitOperatingSystem;
        public string StackTrace;
        public string MainException;
        public string MainExceptionMessage;
        public string InnerException;
        public string Raw;
        public bool Sended;
    }
}
