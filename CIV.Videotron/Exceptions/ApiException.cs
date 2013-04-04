using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Exceptions
{
    public class ApiException : ApplicationException
    {
        public ApiException(string message)
            : base(message)
        {

        }
    }
}
