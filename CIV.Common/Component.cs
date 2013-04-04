using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CIV.Common
{
    public class Component : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; Notify("Name"); }
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { _version = value; Notify("Version"); }
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
