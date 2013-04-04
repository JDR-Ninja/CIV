using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.BOL;
using System.Data.SqlServerCe;
using System.Data;

namespace CIV.DAL
{
    public class CivInfoDAO
    {
        private static CivInfoDAO _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static CivInfoDAO Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = new CivInfoDAO();
                    return _instance;
                }
            }
        }

        public void CreateTable()
        {
            using (SqlCeCommand cmd = new SqlCeCommand("CREATE TABLE CivInfo (name nvarchar(50) NOT NULL, value ntext NOT NULL)", DataBaseFactory.Instance.GetConnection()))
            {
                // Création de la table
                cmd.ExecuteNonQuery();

                // Création de l'index
                cmd.CommandText = "CREATE INDEX idxInfoName ON CivInfo (name);";
                cmd.ExecuteNonQuery();
            }
        }

        private CivInfoBO ReadRecord(SqlCeDataReader reader)
        {
            CivInfoBO result = new CivInfoBO();
            result.Name = reader.GetString(0);
            result.Value = reader.GetString(1);
            return result;
        }

        public void Insert(CivInfoBO civInfoBO)
        {
            using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO CivInfo (name, value) VALUES (@name, @value)", DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@name", SqlDbType.NVarChar) { Value = civInfoBO.Name });
                cmd.Parameters.Add(new SqlCeParameter("@value", SqlDbType.NText) { Value = civInfoBO.Value });
                cmd.ExecuteNonQuery();
            }
        }

        public CivInfoBO Select(string name)
        {
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT TOP 1 * FROM CivInfo WHERE name = @name", DataBaseFactory.Instance.GetConnection()))
            {
                cmd.Parameters.Add(new SqlCeParameter("@name", SqlDbType.NVarChar) { Value = name });

                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        return ReadRecord(dr);
                }
            }

            return null;
        }
    }
}
