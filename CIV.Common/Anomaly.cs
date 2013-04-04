using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class Anomaly
    {
        private Guid _owner;

        public Guid Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        private CIVVersion _version;

        public CIVVersion Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _raw;

        public string Raw
        {
            get { return _raw; }
            set { _raw = value; }
        }

    }
}
