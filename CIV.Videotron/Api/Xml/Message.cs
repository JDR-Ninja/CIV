using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "Message", IsNullable = true)]
    public class Message
    {
        [XmlElementAttribute("code")]
        public string Code { get; set; }

        [XmlElementAttribute("severity")]
        public string Severity { get; set; }

        [XmlElementAttribute("text")]
        public string Text { get; set; }
    }
}