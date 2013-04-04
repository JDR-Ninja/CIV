using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class BackupEndEventArgs : EventArgs
    {
        public bool IsCancelled;

        public BackupEndEventArgs(bool isCancelled)
        {
            IsCancelled = isCancelled;
        }
    }
}
