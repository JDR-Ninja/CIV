using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;

namespace CIV.AppUpdater
{
    public class AppUpdater
    {
        public EventHandler OnUpdateAvailable;
        public EventHandler OnUpdateNotAvailable;

        private bool _updateAvailabled;

        public bool UpdateAvailabled
        {
            get { return _updateAvailabled; }
            set { _updateAvailabled = value; }
        }

        private CIVVersion _currentVersion;

        public CIVVersion CurrentVersion
        {
            get { return _currentVersion; }
            set { _currentVersion = value; }
        }


        public void Execute(string currentVersion)
        {
            CurrentVersion = new CIVVersion(currentVersion);

            CIV.Common.CentralCenterServiceReference.CentralCenterSoapClient client = new Common.CentralCenterServiceReference.CentralCenterSoapClient();

            CIVVersion lastVersion;

            CIV.Common.CentralCenterServiceReference.ReleaseBO lastReleaseBO = client.GetLastVersion("CIV");

            lastVersion = new CIVVersion(lastReleaseBO.Number);

            if (lastVersion.CompareTo(CurrentVersion) == -1)
                OnUpdateNotAvailable(this, null);
            else
                OnUpdateAvailable(this, null);
        }
    }
}
