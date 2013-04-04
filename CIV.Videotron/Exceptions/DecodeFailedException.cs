using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron
{
    public class DecodeFailedException: Exception
    {
        public DecodeFailedTypes Type;
        public string SourcePage;

        public DecodeFailedException(DecodeFailedTypes type, string sourcePage)
        {
            Type = type;
            SourcePage = sourcePage;
        }
    }
}
