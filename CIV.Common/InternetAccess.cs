using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CIV.Common
{
    public class InternetAccess : INotifyPropertyChanged
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; Notify("Id"); }
        }

        private StringMultiLanguage _name;

        public StringMultiLanguage Name
        {
            get { return _name; }
            set { _name = value; Notify("Name"); }
        }

        private double _totalCombined;

        public double TotalCombined
        {
            get { return _totalCombined; }
            set { _totalCombined = value; Notify("TotalCombined"); }
        }

        private double _overCharge;

        public double OverCharge
        {
            get { return _overCharge; }
            set { _overCharge = value; Notify("OverCharge"); }
        }

        private double _uploadSpeed;

        public double UploadSpeed
        {
            get { return _uploadSpeed; }
            set { _uploadSpeed = value; Notify("UploadSpeed"); }
        }

        private double _downloadSpeed;

        public double DownloadSpeed
        {
            get { return _downloadSpeed; }
            set { _downloadSpeed = value; Notify("DownloadSpeed"); }
        }

        private int _maxCost;

        public int MaxCost
        {
            get { return _maxCost; }
            set { _maxCost = value; Notify("MaxCost"); }
        }

        private DateTime _lastUpdate;

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; Notify("LastUpdate"); }
        }

        private string _category;

        public string Category
        {
            get { return _category; }
            set { _category = value; Notify("Category"); }
        }

        public InternetAccess()
        {
            Name = new StringMultiLanguage();
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
