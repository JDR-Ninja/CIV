using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Videotron.Wired;
using Videotron.Enums;

namespace Videotron
{
    internal class WiredAccountCache
    {
        public DateTime Modified { get; set; }

        private WiredAccount _wiredAccount;
        public WiredAccount WiredAccount
        {
            get { return _wiredAccount; }

            set
            {
                _wiredAccount = value;
                Modified = DateTime.Now;
                Status = CacheStatusTypes.Ready;
            }
        }

        public CacheStatusTypes Status { get; set; }

        public WiredAccountCache()
        {
            Modified = DateTime.MinValue;
            Status = CacheStatusTypes.None;
        }
    }
}