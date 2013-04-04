using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron
{
    public class VideotronException : ApplicationException
    {
        public Exception OriginException { get; set; }
        public VideotronExceptionStatus Status { get; set; }

        public VideotronException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
