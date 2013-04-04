using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;
using System.Threading;
using System.Windows.Threading;

namespace CIV
{
    public class AppUpdater
    {
        public delegate void UpdateAvailabledEventHandler(object sender, UpdateAvailabledEventArgs e);

        public event UpdateAvailabledEventHandler OnUpdateAvailable;

        public EventHandler OnUpdateNotAvailable;
        public EventHandler IsNotCompatibleClient;
        public EventHandler OnCompleted;

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
            private set { _currentVersion = value; }
        }

        private CIVVersion _lastVersion;

        public CIVVersion LastVersion
        {
            get { return _lastVersion; }
            private set { _lastVersion = value; }
        }

        public void Execute(string currentVersion)
        {
            CurrentVersion = new CIVVersion(currentVersion);
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            using (CIV.CentralCenterClient.CentralCenterServiceReference.CentralCenterSoapClient client = new CIV.CentralCenterClient.CentralCenterServiceReference.CentralCenterSoapClient())
            {
                if (client.IsCompatibleClient(App.VersionStr()))
                {
                    if (client.IsLastVersion("CIV", CurrentVersion.ToString()))
                        DoUpdateNotAvailable();
                    else
                    {
                        CIV.CentralCenterClient.CentralCenterServiceReference.ReleaseBO lastReleaseBO = client.GetLastVersion("CIV");

                        _lastVersion = new CIVVersion(lastReleaseBO.Number);

                        if (LastVersion.CompareTo(CurrentVersion) > 0)
                        {
                            FileRelease release = new FileRelease()
                            {
                                Number = new CIVVersion(lastReleaseBO.Number),
                                Release = lastReleaseBO.Release,
                                Size = lastReleaseBO.Size,
                                Url = lastReleaseBO.Url,
                                History = lastReleaseBO.History,
                                CRC = lastReleaseBO.CRC
                            };
                            DoUpdateAvailable(new UpdateAvailabledEventArgs(release));
                        }
                        else
                            DoUpdateNotAvailable();
                    }
                }
                else if (IsNotCompatibleClient != null)
                    IsNotCompatibleClient(this, null);
            }
        }

        private void DoUpdateAvailable(UpdateAvailabledEventArgs e)
        {
            DoCompleted();
            if (OnUpdateAvailable != null)
                OnUpdateAvailable(this, e);
        }

        private void DoUpdateNotAvailable()
        {
            DoCompleted();
            if (OnUpdateNotAvailable != null)
                OnUpdateNotAvailable(this, null);
        }

        private void DoCompleted()
        {
            if (OnCompleted != null)
                OnCompleted(this, null);
        }
    }
}
