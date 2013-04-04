using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CIV
{
    public class SmtpSettings : INotifyPropertyChanged
    {
        private bool _active;

        public bool Active
        {
            get { return _active; }
            set { _active = value; Notify("Active"); }
        }

        private string _host;

        public string Host
        {
            get { return _host; }
            set { _host = value; Notify("Host"); }
        }

        private int _port;

        public int Port
        {
            get { return _port; }
            set { _port = value; Notify("Port"); }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; Notify("Username"); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; Notify("Password"); }
        }

        private string _sender;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; Notify("Sender"); }
        }

        private string _senderMail;

        public string SenderMail
        {
            get { return _senderMail; }
            set { _senderMail = value; Notify("SenderMail"); }
        }


        public SmtpSettings()
        {
            Host = "relais.videotron.ca";
            Port = 25;
            Username = "VLxxxx";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
