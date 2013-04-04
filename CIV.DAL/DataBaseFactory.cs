using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;
using System.Data;
using CIV.Common;
using CIV.BOL;

namespace CIV.DAL
{
    public class DataBaseFactory
    {
        private static CIVVersion _version = new CIVVersion("3.1.0");
        public bool IsAvailable;

        private SqlCeConnection _sqlConnection = null;
        private static DataBaseFactory _instance;
        private static object _instanceLocker = new object();

        public static DataBaseFactory Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                    {
                        _instance = new DataBaseFactory();
                        _instance.IsAvailable = true;
                        _instance.DBName = Path.Combine(CIV.Common.IO.GetCivDataFolder(), "CIVData.sdf");
                        _instance.ConnectionString = string.Format("DataSource='{0}';Password='Wz3JtttRFmdquUd8I6FaiuxdPrhl9w0V2kOk8';Encrypt=TRUE;Max Database Size=1024;", _instance.DBName);
                    }
                    return _instance;
                }
            }
        }

        private string _dbName;
        public string DBName { get { return _dbName; } set { _dbName = value; } }
        private string _connectionString;
        public string ConnectionString { get { return _connectionString; } set { _connectionString = value; } }

        /// <summary>
        /// Retourne l'objet de connection de la BD
        /// </summary>
        /// <returns></returns>
        public SqlCeConnection GetConnection()
        {
            if (_sqlConnection == null)
                _sqlConnection = new SqlCeConnection(_connectionString);

            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            return _sqlConnection;
        }

        /// <summary>
        /// Compacte la base de donnée
        /// </summary>
        /// <param name="compactException"></param>
        /// <returns></returns>
        public bool Compact(out Exception compactException)
        {
            compactException = null;
            GetConnection().Close();
            try
            {
                using (SqlCeEngine engine = new SqlCeEngine(ConnectionString))
                {
                    engine.Compact(null);
                    return true;
                }
            }
            catch (Exception innerCompactException)
            {
                compactException = innerCompactException;
                IsAvailable = false;
            }
            return false;
        }

        /// <summary>
        /// Répare la BD
        /// </summary>
        /// <param name="repairException"></param>
        /// <returns></returns>
        public bool Repair(out Exception repairException)
        {
            repairException = null;

            try
            {
                GetConnection().Close();
                using (SqlCeEngine engine = new SqlCeEngine(ConnectionString))
                {
                    engine.Repair(null, RepairOption.DeleteCorruptedRows);
                    return true;
                }
            }
            catch (Exception innerRepairException)
            {
                repairException = innerRepairException;
            }
            return false;
        }

        /// <summary>
        /// Vérifie si la BD existe et la mise à jours au besoin
        /// </summary>
        public void CheckDatabase()
        {
            if (IsAvailable)
            {
                if (!File.Exists(DBName))
                    CreateDatabase();
                else
                {
                    // 3.1.0
                    if (!TableExist("CivInfo"))
                    {
                        // Création de la table contenant la version de la BD
                        CreateCIVInfoTable();

                        // Remplir les valeurs par défaut
                        foreach (string user in GetUserTables())
                        {
                            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("ALTER TABLE {0} ADD period nChar(17)", user), DataBaseFactory.Instance.GetConnection()))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            DailyUsageDAO.Instance.FillPeriod(user);
                        }
                    }
                }
            }
        }

        public bool ResetDatabase()
        {
            if (File.Exists(DBName))
            {
                GetConnection().Close();
                try
                {
                    // Premièrement, on tente de détuire le fichier
                    File.Delete(DBName);
                    CreateDatabase();
                    return true;
                }
                catch
                {
                    // Si ça ne fonctionne pas, on essai de faire des DROP TABLE
                    foreach (string tablename in GetUserTables())
                        DropTable(tablename);
                    CreateDatabase();
                    return true;
                }
            }
            return false;
        }

        public void DropTable(string tablename)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(String.Format("DROP TABLE {0}", tablename), DataBaseFactory.Instance.GetConnection()))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public bool TableExist(string tablename)
        {
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TABLE", DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@TABLE", SqlDbType.NVarChar) { Value = tablename });

                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read();
                        
                }
            }
        }

        public void CreateDatabase()
        {
            SqlCeEngine engine = new SqlCeEngine(ConnectionString);
            engine.CreateDatabase();
            CreateCIVInfoTable();
        }

        private void CreateCIVInfoTable()
        {
            if (IsAvailable)
            {
                CivInfoDAO.Instance.CreateTable();

                CivInfoBO info = new CivInfoBO();
                info.Name = "Version";
                info.Value = XmlFactory.SaveToString(_version, typeof(CIVVersion), new XmlFactorySettings());

                CivInfoDAO.Instance.Insert(info);
            }
        }

        public List<string> GetUserTables()
        {
            List<string> result = new List<string>();

            using (SqlCeCommand cmd = new SqlCeCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME", DataBaseFactory.Instance.GetConnection()))
            {
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        if (dr.GetString(0).Substring(0, 2) == "VL")
                            result.Add(dr.GetString(0));
                }
            }            

            return result;
        }
    }
}
