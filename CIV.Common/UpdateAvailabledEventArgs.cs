using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class UpdateAvailabledEventArgs : EventArgs
    {
        private FileRelease _release;

        public FileRelease Release
        {
            get { return _release; }
            set { _release = value; }
        }

        public UpdateAvailabledEventArgs(FileRelease release)
        {
            Release = release;
        }
    }
}
