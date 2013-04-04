using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class CivException
    {
        public string ExceptionType;
        public string Message;
        public string Source;
        public string StackTrace;
        public CivException InnerException = null;

        public CivException()
        {

        }

        public CivException(Exception exception)
        {
            ExceptionType = exception.GetType().ToString();
            Message = exception.Message;
            StackTrace = exception.StackTrace;

            if (exception.InnerException != null)
                InnerException = new CivException(exception.InnerException);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(String.Format("ExceptionType :\t{0}", ExceptionType));
            result.AppendLine(String.Format("Message :\t{0}", Message));
            result.AppendLine(String.Format("Source :\t{0}", Source));
            result.AppendLine(String.Format("StackTrace :\t{0}", StackTrace));
            
            if (InnerException != null)
                result.AppendLine(String.Format("InnerException :\t{0}", InnerException));

            return result.ToString();
        }
    }
}
