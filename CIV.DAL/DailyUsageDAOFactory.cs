using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.BOL;
using System.Xml.Serialization;
using System.IO;
using CIV.Common;

namespace CIV.DAL
{
    [XmlRootAttribute(ElementName = "InternetUsages", IsNullable = false)]
    public class DailyUsageDAOFactory : List<DailyUsageExportBO>
    {
        private string _password = "y3XfjCr7q9gXJGiLJtOO";

        public bool Export(string filename, out Exception exportException, out int count)
        {
            exportException = null;
            count = 0;
            Clear();
            try
            {
                foreach (string tablename in DataBaseFactory.Instance.GetUserTables())

                    foreach (DailyUsageBO usage in DailyUsageDAO.Instance.All(tablename))
                    {
                        Add(new DailyUsageExportBO(usage, tablename));
                        count++;
                    }
                XmlFactory.SaveToFile(this, typeof(DailyUsageDAOFactory), filename, new XmlFactorySettings() { Password = _password });

                return true;
            }
            catch (Exception innerExportException)
            {
                exportException = innerExportException;
                return false;
            }
        }

        public bool Import(string filename, out Exception importException, out int count, out int total)
        {
            importException = null;
            count = 0;
            total = 0;
            try
            {
                DailyUsageDAOFactory data = (DailyUsageDAOFactory)XmlFactory.LoadFromFile(typeof(DailyUsageDAOFactory), filename, new XmlFactorySettings() { Password = _password });

                List<string> tables = DataBaseFactory.Instance.GetUserTables();

                foreach (DailyUsageExportBO usage in data)
                {
                    if (!tables.Contains(usage.Username))
                        DailyUsageDAO.Instance.CreateTable(usage.Username);

                    if (!DailyUsageDAO.Instance.Exist(usage.Username, new DailyUsageBO() { Day = usage.Day }))
                    {
                        if (usage.Period == null)
                        {
                            usage.Period = new Period(String.Format("{0}01@{1}{2}",
                                                      usage.Day.ToString("yyyyMM"),
                                                      usage.Day.ToString("yyyyMM"),
                                                      DateTime.DaysInMonth(Convert.ToInt32(usage.Day.ToString("yyyy")),
                                                                           Convert.ToInt32(usage.Day.ToString("MM")))
                                                      ));
                        }

                        DailyUsageDAO.Instance.Insert(usage.Username, usage);
                        count++;
                    }
                    total++;
                }

                return true;
            }
            catch (Exception innerImportException)
            {
                importException = innerImportException;
                return false;
            }
        }
    }
}
