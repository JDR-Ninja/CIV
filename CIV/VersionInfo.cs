using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;

namespace CIV
{
    [Serializable]
    public class VersionInfo
    {
        /// <summary>
        /// Le numéro de la version
        /// </summary>
        private CIVVersion _number;

        public CIVVersion Number
        {
            get { return _number; }
            set { _number = value; }
        }

        /// <summary>
        /// La date de sortie de la version
        /// </summary>
        private DateTime _release;

        public DateTime Release
        {
            get { return _release; }
            set { _release = value; }
        }

        /// <summary>
        /// Le lien pour télécharger la version
        /// </summary>
        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// La taille du fichier
        /// </summary>
        private double _size;

        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }

        private string _history;

        public string History
        {
            get { return _history; }
            set { _history = value; }
        }
    }
}
