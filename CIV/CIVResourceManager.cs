using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace CIV
{
    public class CIVResourceManager : INotifyPropertyChanged
    {
        private static CIV.strings _textResources = new CIV.strings();

        public CIV.strings Text
        {
            get { return _textResources; }
            set { OnPropertyChanged("Text"); }
        }

        public void Reload()
        {
            _textResources = new CIV.strings();
            OnPropertyChanged("Text");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
