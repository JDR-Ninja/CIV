using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CIV
{
    public class BillPeriod : INotifyPropertyChanged
    {
        private DateTime _start;

        public DateTime Start
        {
            get { return _start; }
            set { _start = value; Notify("Start"); }
        }

        private DateTime _end;

        public DateTime End
        {
            get { return _end; }
            set { _end = value; Notify("End"); }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; Notify("Text"); }
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
