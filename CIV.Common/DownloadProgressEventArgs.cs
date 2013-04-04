using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public FileDownload File;
        public long Downloaded;

        public DownloadProgressEventArgs(FileDownload file, long downloaded)
        {
            File = file;
            Downloaded = downloaded;
        }
    }
}
