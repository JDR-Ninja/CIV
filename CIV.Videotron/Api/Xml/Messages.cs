using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Videotron.Api.Xml
{
    [XmlRootAttribute(ElementName = "Message", IsNullable = true)]
    public class Messages
    {
        [XmlElementAttribute("Message")]
        public List<Message> Message { get; set; }

        public Messages()
        {
            Message = new List<Message>();
        }
    }
}
