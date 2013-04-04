using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using CIV.BOL;
using System.Globalization;
using CIV.Common;

namespace CIV.DAL
{
    public class DailyUsageDAO
    {
        private static DailyUsageDAO _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static DailyUsageDAO Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = new DailyUsageDAO();
                    return _instance;
                }
            }
        }

        public void CreateTable(string username)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("CREATE TABLE {0} (day datetime NOT NULL PRIMARY KEY, month nchar(6) NOT NULL, upload float NOT NULL, download float NOT NULL, total float NOT NULL, period nchar(17))", username), DataBaseFactory.Instance.GetConnection()))
            {
                // Création de la table
                cmd.ExecuteNonQuery();

                // Création de l'index
                cmd.CommandText = String.Format("CREATE INDEX idxMonth ON {0} (month);", username);
                cmd.ExecuteNonQuery();

                cmd.CommandText = String.Format("CREATE INDEX idxPeriod ON {0} (period);", username);
                cmd.ExecuteNonQuery();
            }
        }

        private DailyUsageBO ReadRecord(SqlCeDataReader reader)
        {
            DailyUsageBO result = new DailyUsageBO();
            result.Day = reader.GetDateTime(0);
            result.Month = reader.GetString(1);
            result.Upload = reader.GetDouble(2);
            result.Download = reader.GetDouble(3);
            result.Total = reader.GetDouble(4);
            if (!reader.IsDBNull(5))
                result.Period = new Period(reader.GetString(5));
            return result;
        }

        public bool Exist(string username, DailyUsageBO usage)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT * FROM {0} WHERE day = @day", username),
                                                        DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@day", SqlDbType.DateTime) { Value = usage.Day });

                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read();
                }
            }
        }

        public bool Exist(string username, DailyUsageBO usage, out DailyUsageBO data)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT * FROM {0} WHERE day = @day", username),
                                                        DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@day", SqlDbType.DateTime) { Value = usage.Day });

                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        data = ReadRecord(dr);
                        return true;
                    }
                }
            }
            data = null;
            return false;
        }

        public void Update(string username, DailyUsageBO dailyUsage)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("UPDATE {0} SET upload=@upload, download=@download, total=@total WHERE day=@day", username),
                                                                DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@upload", SqlDbType.Float) { Value = dailyUsage.Upload });
                cmd.Parameters.Add(new SqlCeParameter("@download", SqlDbType.Float) { Value = dailyUsage.Download });
                cmd.Parameters.Add(new SqlCeParameter("@total", SqlDbType.Float) { Value = dailyUsage.Total });
                cmd.Parameters.Add(new SqlCeParameter("@day", SqlDbType.DateTime) { Value = dailyUsage.Day });
                cmd.ExecuteNonQuery();
            }
        }

        public void Insert(string username, DailyUsageBO dailyUsage)
        {
            try
            {
                if (dailyUsage.Total != 0)
                {
                    using (SqlCeCommand cmd = new SqlCeCommand(String.Format("INSERT INTO {0} (day, month, upload, download, total, period) VALUES (@day, @month, @upload, @download, @total, @period)", username),
                                                               DataBaseFactory.Instance.GetConnection()))
                    {
                        cmd.Parameters.Add(new SqlCeParameter("@day", SqlDbType.DateTime) { Value = dailyUsage.Day });
                        cmd.Parameters.Add(new SqlCeParameter("@month", SqlDbType.NChar) { Value = dailyUsage.Day.Year.ToString() + dailyUsage.Day.Month.ToString().PadLeft(2, '0') });
                        cmd.Parameters.Add(new SqlCeParameter("@upload", SqlDbType.Float) { Value = dailyUsage.Upload });
                        cmd.Parameters.Add(new SqlCeParameter("@download", SqlDbType.Float) { Value = dailyUsage.Download });
                        cmd.Parameters.Add(new SqlCeParameter("@total", SqlDbType.Float) { Value = dailyUsage.Total });
                        cmd.Parameters.Add(new SqlCeParameter("@period", SqlDbType.NChar) { Value = dailyUsage.Period.ToString() });
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception sql)
            {
                if (!DataBaseFactory.Instance.TableExist(username))
                {
                    CreateTable(username);

                    using (SqlCeCommand cmd = new SqlCeCommand(String.Format("INSERT INTO {0} (day, month, upload, download, total, period) VALUES (@day, @month, @upload, @download, @total, @period)", username),
                                                                   DataBaseFactory.Instance.GetConnection()))
                    {
                        cmd.Parameters.Add(new SqlCeParameter("@day", SqlDbType.DateTime) { Value = dailyUsage.Day });
                        cmd.Parameters.Add(new SqlCeParameter("@month", SqlDbType.NChar) { Value = dailyUsage.Day.Year.ToString() + dailyUsage.Day.Month.ToString().PadLeft(2, '0') });
                        cmd.Parameters.Add(new SqlCeParameter("@upload", SqlDbType.Float) { Value = dailyUsage.Upload });
                        cmd.Parameters.Add(new SqlCeParameter("@download", SqlDbType.Float) { Value = dailyUsage.Download });
                        cmd.Parameters.Add(new SqlCeParameter("@total", SqlDbType.Float) { Value = dailyUsage.Total });
                        cmd.Parameters.Add(new SqlCeParameter("@period", SqlDbType.NChar) { Value = dailyUsage.Period.ToString() });
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                    throw sql;
            }
            
        }

        public List<Period> AllPeriod(string username)
        {
            List<Period> result = new List<Period>();

            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT DISTINCT(period) FROM {0} ORDER BY period DESC", username), DataBaseFactory.Instance.GetConnection()))
            {
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        result.Add(new Period(dr.GetString(0)));
                    }
                }
            }

            return result;
        }

        public List<DailyUsageBO> UsageByPeriod(string username, DateTime start, DateTime end)
        {
            List<DailyUsageBO> result = new List<DailyUsageBO>();

            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                try
                {
                    using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT * FROM {0} WHERE day BETWEEN @start AND @end ORDER BY day", username),
                                                               DataBaseFactory.Instance.GetConnection()))
                    {
                        cmd.Parameters.Add(new SqlCeParameter("@start", SqlDbType.DateTime) { Value = start });
                        cmd.Parameters.Add(new SqlCeParameter("@end", SqlDbType.DateTime) { Value = end });

                        using (SqlCeDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                                result.Add(ReadRecord(dr));
                        }
                    }
                }
                catch
                {
                    CreateTable(username);
                }
            }

            return result;
        }

        public List<DailyUsageBO> All(string username)
        {
            List<DailyUsageBO> result = new List<DailyUsageBO>();

            try
            {
                using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT * FROM {0} ORDER BY day", username),
                                                           DataBaseFactory.Instance.GetConnection()))
                {
                    using (SqlCeDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            result.Add(ReadRecord(dr));
                    }
                }
            }
            catch
            {
                CreateTable(username);
            }

            return result;
        }

        public List<DailyUsageBO> AllByMonth(string username)
        {
            List<DailyUsageBO> result = new List<DailyUsageBO>();
            DailyUsageBO record;
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT month, SUM(upload), SUM(download), SUM(total) FROM {0} GROUP BY month ORDER BY month", username),
                                                           DataBaseFactory.Instance.GetConnection()))
                {
                    using (SqlCeDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            record = new DailyUsageBO();
                            record.Month = dr.GetString(0);
                            record.Upload = dr.GetDouble(1);
                            record.Download = dr.GetDouble(2);
                            record.Total = dr.GetDouble(3);
                            record.Day = DateTime.ParseExact(record.Month, "yyyyMM", provider);
                            result.Add(record);
                        }
                    }
                }
            }
            catch
            {
                CreateTable(username);
            }

            return result;
        }

        public void FillPeriod(string username)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("SELECT * FROM {0} WHERE period IS NULL", username), DataBaseFactory.Instance.GetConnection()))
            {
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DailyUsageBO record = ReadRecord(dr);
                        using (SqlCeCommand cmdUpd = new SqlCeCommand(String.Format("UPDATE {0} SET period=@period WHERE day=@day", username), DataBaseFactory.Instance.GetConnection()))
                        {
                            cmdUpd.Parameters.Add(new SqlCeParameter("@day", SqlDbType.DateTime) { Value = record.Day });
                            cmdUpd.Parameters.Add(new SqlCeParameter("@period", SqlDbType.NChar)
                            {
                                Value = String.Format("{0}01@{1}{2}",
                                                      record.Day.ToString("yyyyMM"),
                                                      record.Day.ToString("yyyyMM"),
                                                      DateTime.DaysInMonth(Convert.ToInt32(record.Day.ToString("yyyy")),
                                                                           Convert.ToInt32(record.Day.ToString("MM")))
                                                      )
                            });
                            cmdUpd.ExecuteNonQuery();
                        }

                    }
                }
            }
        }
    }
}
