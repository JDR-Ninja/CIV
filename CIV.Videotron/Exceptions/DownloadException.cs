using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Exceptions
{
    public class DownloadException : VideotronException
    {
        public DownloadException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
