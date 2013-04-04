using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class FileRelease
    {
        private DateTime _release;

        public DateTime Release
        {
            get { return _release; }
            set { _release = value; }
        }

        private CIVVersion _number;

        public CIVVersion Number
        {
            get { return _number; }
            set { _number = value; }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private int _size;

        public int Size
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

        private string _crc;

        public string CRC
        {
            get { return _crc; }
            set { _crc = value; }
        }
    }
}
