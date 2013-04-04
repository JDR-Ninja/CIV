using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CIV.Common;

namespace LogFactory
{
    public class LogElementBO
    {
        public Guid Owner;
        public DateTime Created;
        public string OsVersion;
        public CivException Error;
        //public CivCultureInfo Culture;
        //public CivCultureInfo UICulture;
        public string ApplicationPath;
        public string CivVersion;
        public string Raw;
        public string[] Assembly;

        public LogElementBO()
        {
            Created = DateTime.Now;

            //Culture = new CivCultureInfo(Thread.CurrentThread.CurrentCulture);
            //UICulture = new CivCultureInfo(Thread.CurrentThread.CurrentUICulture);

            OsVersion = String.Format(Environment.Is64BitOperatingSystem ? "{0} {1} 64bits" : "{0} {1} 32bits", Environment.OSVersion.Version, Environment.OSVersion.ServicePack);

            ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(String.Format("Date :\t{0}", Created.ToString("yyyy-MM-dd HH:mm:ss")));
            result.AppendLine(String.Format("OsVersion :\t{0}", OsVersion));
            result.AppendLine(String.Format("Error :\t{0}", Error));
            //result.AppendLine(String.Format("Culture :\t{0}", Culture));
            //result.AppendLine(String.Format("UICulture :\t{0}", UICulture));
            result.AppendLine(String.Format("ApplicationPath :\t{0}", ApplicationPath));
            result.AppendLine(String.Format("CivVersion :\t{0}", CivVersion));
            result.AppendLine(String.Format("Raw :\t{0}", Raw));

            return result.ToString();
        }
    }
}
