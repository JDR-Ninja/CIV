using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public enum ScreenMessageType { Normal, Exception }

    public class ScreenMessage
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private DateTime _created;

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        private ScreenMessageType _messageType;

        public ScreenMessageType MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        public ScreenMessage()
        {
            Created = DateTime.Now;
            MessageType = ScreenMessageType.Normal;
        }

        public ScreenMessage(string title, string text, ScreenMessageType messageType)
        {
            Created = DateTime.Now;
            MessageType = ScreenMessageType.Normal;
            Title = title;
            Text = text;
            MessageType = messageType;
        }
    }
}
