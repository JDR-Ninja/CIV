using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using CIV.Common;
using System.Reflection;

namespace LogFactory
{
    public class LogEngine
    {
        private static LogEngine _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static LogEngine Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = new LogEngine();
                    return _instance;
                }
            }
        }

        public string ClientVersion;
        public Guid Owner;
        public Boolean AutomaticSendReport;

        public void Add(Exception exception, [Optional, DefaultParameterValue(true)] bool sendtoWeb)
        {
            if (exception is DllNotFoundException)
            {
                if (exception.Message.IndexOf("sqlceme35.dll") < 0)
                    Add(new LogElementBO() { Error = new CivException(exception) }, sendtoWeb);
            }
            else
                Add(new LogElementBO() { Error = new CivException(exception)}, sendtoWeb);
        }

        public void Add(LogElementBO element, [Optional, DefaultParameterValue(true)] bool sendtoWeb)
        {
            element.Owner = Owner;
            element.CivVersion = ClientVersion;

            element.Assembly = new string[AppDomain.CurrentDomain.GetAssemblies().Length];
            int i = 0;
            foreach (Assembly MyAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                element.Assembly[i] = MyAssembly.ToString();
                i++;
            }

            WriteToFile(element);

            // Désactivation des services web
            //if (AutomaticSendReport && sendtoWeb)
            //{
            //    ThreadStart start = delegate()
            //    {
            //        SendToWeb(element);
            //    };
            //    new Thread(start).Start();
            //}
        }

        //private void SendToWeb(LogElementBO element)
        //{
        //    try
        //    {
        //        CIV.CentralCenterClient.CentralCenterServiceReference.ReportBO report = new CIV.CentralCenterClient.CentralCenterServiceReference.ReportBO();
        //        report.Owner = element.Owner;
        //        report.Created = element.Created;
        //        report.CivVersion = element.CivVersion;
        //        report.ExceptionType = element.Error.ExceptionType;
        //        report.LogElementBOSerialized = CIV.Common.IO.GZipCompressString(XmlFactory.SaveToString(element, typeof(LogElementBO), new XmlFactorySettings()), new UTF8Encoding());

        //        CIV.CentralCenterClient.CentralCenterServiceReference.CentralCenterSoapClient centralCenter = new CIV.CentralCenterClient.CentralCenterServiceReference.CentralCenterSoapClient();
                
        //        if (centralCenter.IsCompatibleClient(ClientVersion) && centralCenter.IsLastVersion("CIV", report.CivVersion))
        //            centralCenter.SendReport(report);
        //    }
        //    catch (Exception sendError)
        //    {
        //        WriteToFile(new LogElementBO() { Error = new CivException(sendError), Created = DateTime.Now, OsVersion = Environment.OSVersion.Version.ToString() });
        //    }
        //}

        private void WriteToFile(LogElementBO element)
        {
            try
            {
                //Exception innerException;

                using (StreamWriter logFile = new StreamWriter(Path.Combine(CIV.Common.IO.GetCivDataFolder(), "civ.log"), true))
                {
                    //logFile.AutoFlush = true;
                    logFile.WriteLine("====================================================================================================");
                    logFile.WriteLine(element);
                    logFile.Flush();
                }
            }
            catch { }
        }
    }
}
