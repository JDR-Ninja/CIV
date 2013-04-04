using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class FileDownload
    {
        private string _url;

        /// <summary>
        /// Le lien web (http) où est situé le fichier
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _filename;

        /// <summary>
        /// Le nom complet du fichier où il doit être sauvegardé
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        private long _size;

        /// <summary>
        /// La taille du fichier. Cette valeur est réinitialisé lors du contact du serveur web
        /// </summary>
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }

        private string _temp;

        /// <summary>
        /// Le nom du fichier temporaire
        /// </summary>
        public string Temp
        {
            get { return _temp; }
            set { _temp = value; }
        }

    }
}
