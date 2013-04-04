using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using CIV.BOL;

namespace CIV.DAL
{
    public class LogEventDAO
    {
        private static LogEventDAO _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static LogEventDAO Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = new LogEventDAO();
                    return _instance;
                }
            }
        }

        public void Insert(LogEvent logEvent)
        {
            using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO CIV_LOG (Id, Created, OsVersion, Is64BitOperatingSystem, StackTrace, MainException, MainExceptionMessage, InnerException, Raw, Sended) VALUES (@Id, @Created, @OsVersion, @Is64BitOperatingSystem, @StackTrace, @MainException, @MainExceptionMessage, @InnerException, @Raw, @Sended)", DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@Id", SqlDbType.UniqueIdentifier) { Value = logEvent.Id });
                cmd.Parameters.Add(new SqlCeParameter("@Created", SqlDbType.DateTime) { Value = logEvent.Created });
                cmd.Parameters.Add(new SqlCeParameter("@OsVersion", SqlDbType.NVarChar) { Value = logEvent.OsVersion });
                cmd.Parameters.Add(new SqlCeParameter("@Is64BitOperatingSystem", SqlDbType.Bit) { Value = logEvent.Is64BitOperatingSystem });
                cmd.Parameters.Add(new SqlCeParameter("@StackTrace", SqlDbType.NVarChar) { Value = logEvent.StackTrace });
                cmd.Parameters.Add(new SqlCeParameter("@MainException", SqlDbType.NVarChar) { Value = logEvent.MainException });
                cmd.Parameters.Add(new SqlCeParameter("@MainExceptionMessage", SqlDbType.NVarChar) { Value = logEvent.MainExceptionMessage });
                cmd.Parameters.Add(new SqlCeParameter("@InnerException", SqlDbType.NText) { Value = logEvent.InnerException });
                cmd.Parameters.Add(new SqlCeParameter("@Raw", SqlDbType.NText) { Value = logEvent.Raw });
                cmd.Parameters.Add(new SqlCeParameter("@Sended", SqlDbType.Bit) { Value = logEvent.Raw });
                cmd.ExecuteNonQuery();
            }
        }
    }
}
