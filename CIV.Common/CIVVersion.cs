using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class CIVVersion
    {
        private int _major = 0;

        public int Major
        {
            get { return _major; }
            set { _major = value; }
        }

        private int _minor = 0;

        public int Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        private int _build = 0;

        public int Build
        {
            get { return _build; }
            set { _build = value; }
        }

        private int _revision = 0;

        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        public CIVVersion()
        {
            
        }

        /// <summary>
        /// Constructeur qui initialise l'objet avec une version
        /// </summary>
        /// <param name="text">Texte représentant un numéro de version. Exemple: "1.0.0.0"</param>
        public CIVVersion(string text)
        {
            string[] version = text.Split('.');

            if (version.Length > 0)
            {
                _major = Convert.ToInt32(version[0]);

                if (version.Length > 1)
                {
                    _minor = Convert.ToInt32(version[1]);

                    if (version.Length > 2)
                    {
                        _build = Convert.ToInt32(version[2]);

                        if (version.Length > 3)
                        {
                            _revision = Convert.ToInt32(version[3]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compare 2 versions
        /// </summary>
        /// <param name="version">La version à comparer</param>
        /// <returns>-1 si l'instance est inférieur, 0 s'ils sont égalent, 1 si l'instance est supérieur</returns>
        public int CompareTo(CIVVersion version)
        {
            // Majeur
            if (Major < version.Major)
                return -1;

            else if (Major > version.Major)
                return 1;

            else
            {
                // Mineur
                if (Minor < version.Minor)
                    return -1;

                else if (Minor > version.Minor)
                    return 1;

                else
                {
                    // Build
                    if (Build < version.Build)
                        return -1;

                    else if (Build > version.Build)
                        return 1;

                    else
                    {
                        // Révision
                        if (Revision < version.Revision)
                            return -1;

                        else if (Revision > version.Revision)
                            return 1;

                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            if (Revision != 0)
                return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
            else
                return String.Format("{0}.{1}.{2}", Major, Minor, Build);
        }
    }
}
