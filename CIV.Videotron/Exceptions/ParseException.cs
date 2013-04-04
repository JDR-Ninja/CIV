using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Exceptions
{
    public class ParseException : VideotronException
    {
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
