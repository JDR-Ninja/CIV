using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Wired
{
    public class WiredMessage
    {
        public WiredMessageCodeTypes Code { get; set; }
        public WiredMessageSeverityTypes Severity { get; set; }
        public string Text { get; set; }
    }
}
