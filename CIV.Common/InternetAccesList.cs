using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace CIV.Common
{
    [XmlRootAttribute(ElementName = "InternetAccesList", IsNullable = false)]
    public class InternetAccesList
    {
        private const string _filename = "InternetAccessList.xml";
        private static InternetAccesList _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static InternetAccesList Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = InternetAccesList.Load();
                    return _instance;
                }
            }
        }

        private List<InternetAccess> _access;

        public static void Reload()
        {
            _instance = Load();
        }

        [XmlElementAttribute("InternetAccess")]
        public List<InternetAccess> Access
        {
            get { return _access; }
        }
        
        public InternetAccesList()
        {
            _access = new List<InternetAccess>();
        }

        public InternetAccess this[string code]
        {
            get
            {
                foreach (InternetAccess access in _access)
                    if (access.Id == code)
                        return access;

                return null;
            }
            private set{}
        }

        public static InternetAccesList Load()
        {
            InternetAccesList result = new InternetAccesList();

            if (File.Exists(Filename))
                return (InternetAccesList)XmlFactory.LoadFromFile(typeof(InternetAccesList), Filename, new XmlFactorySettings());

            return result;
        }

        public void Save()
        {
            XmlFactory.SaveToFile(this, typeof(InternetAccesList), Filename, new XmlFactorySettings());
        }

        public static string Filename
        {
            get { return Path.Combine(CIV.Common.IO.GetCivDataFolder(), _filename); }
        }
    }
}