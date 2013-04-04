using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CIV.Common;
using System.Windows.Media.Animation;
using System.Net;
using System.Net.Cache;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using ICSharpCode.SharpZipLib.Checksums;

namespace CIV
{
    /// <summary>
    /// Interaction logic for Updater.xaml
    /// </summary>
    public partial class Updater : Window
    {
        private enum UpdaterState { None, Availabled, Destination, Download, Install }
        private UpdaterState _state;
        private string _setup;

        private FileDownloadManager downloader = new FileDownloadManager();

        private FileRelease _release;

        public FileRelease Release
        {
            get { return _release; }
            private set { _release = value; }
        }

        public Updater()
        {
            InitializeComponent();
            this.Title = strings.Updater_Title;
            _state = UpdaterState.None;

            downloader.OnDownloadProgress += OnDownloadProgress;
            downloader.UserAgent = "CIV " + App.VersionLongStr();
        }

        public void Execute(FileRelease release)
        {
            Release = release;
            lblVersion.Text = release.Number.ToString();
            lblRelease.Text = release.Release.ToString("d MMMM yyyy");
            lblSize.Text = String.Format("{0:N2} ko", (double)release.Size / 1024);
            txtHistory.Text = release.History;
            txtSavePath.Text = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            NextStep();
            Show();
        }

        private void NextStep()
        {
            switch (_state)
            {
                case UpdaterState.None:
                    _state = UpdaterState.Availabled;
                    btnPrior.IsEnabled = false;
                    btnNext.Visibility = System.Windows.Visibility.Visible;
                    btnInstall.Visibility = System.Windows.Visibility.Collapsed;
                    tiAvailabled.IsEnabled = true;
                    tiAvailabled.IsSelected = true;
                    tiDestination.IsEnabled = false;
                    tiDownload.IsEnabled = false;
                    tiInstallation.IsEnabled = false;
                    pbDownload.BeginAnimation(ProgressBar.ValueProperty, null);
                    btnNext.Focus();
                    break;
                case UpdaterState.Availabled:
                    _state = UpdaterState.Destination;
                    btnPrior.IsEnabled = true;
                    btnNext.Visibility = System.Windows.Visibility.Visible;
                    btnInstall.Visibility = System.Windows.Visibility.Collapsed;
                    tiAvailabled.IsEnabled = false;
                    tiDestination.IsEnabled = true;
                    tiDestination.IsSelected = true;
                    tiDownload.IsEnabled = false;
                    tiInstallation.IsEnabled = false;
                    pbDownload.BeginAnimation(ProgressBar.ValueProperty, null);
                    btnNext.Focus();
                    break;
                case UpdaterState.Destination:
                    // Validation avant de download
                    _state = UpdaterState.Download;
                    btnPrior.IsEnabled = true;
                    btnNext.Visibility = System.Windows.Visibility.Collapsed;
                    btnInstall.Visibility = System.Windows.Visibility.Visible;
                    tiInstallation.IsEnabled = false;
                    txtDwlError.Visibility = System.Windows.Visibility.Collapsed;
                    tiAvailabled.IsEnabled = false;
                    tiDestination.IsEnabled = false;
                    tiDownload.IsEnabled = true;
                    tiDownload.IsSelected = true;
                    txtDwlError.Visibility = System.Windows.Visibility.Collapsed;

                    tiDownload.UpdateLayout();
                    Download();

                    break;

                // Téléchargement terminé, on peut installer
                case UpdaterState.Download:
                    _state = UpdaterState.Install;
                    //btnPrior.IsEnabled = true;
                    btnPrior.Visibility = System.Windows.Visibility.Collapsed;
                    btnNext.Visibility = System.Windows.Visibility.Collapsed;
                    btnInstall.Visibility = System.Windows.Visibility.Visible;
                    btnInstall.IsEnabled = true;
                    tiAvailabled.IsEnabled = false;
                    tiDestination.IsEnabled = false;
                    tiDownload.IsEnabled = false;
                    tiInstallation.IsEnabled = true;
                    tiInstallation.IsSelected = true;
                    btnInstall.Focus();
                    break;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            NextStep();
        }

        private void btnPrior_Click(object sender, RoutedEventArgs e)
        {
            switch (_state)
            {
                case UpdaterState.Availabled:
                    _state = UpdaterState.None;
                    
                    break;
                case UpdaterState.Destination:
                    _state = UpdaterState.None;
                    
                    break;
                case UpdaterState.Download:
                    _state = UpdaterState.Availabled;
                    downloader.Abort = true;
                    break;
                case UpdaterState.Install:
                    _state = UpdaterState.Destination;
                    break;
            }
            NextStep();
        }

        private void btnSavePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dia = new System.Windows.Forms.FolderBrowserDialog();
            dia.RootFolder = System.Environment.SpecialFolder.MyComputer;

            if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtSavePath.Text = dia.SelectedPath;
        }

        private void Download()
        {
            //_release.Url = "http://malealpha.com/dwl/tinyfile.dat";
            //_release.Url = "http://malealpha.com/dwl/bigfile.dat";
            //_release.Url = "http://malealpha.com/dwl/bigfile.dats";
            try
            {
                downloader.Execute(new FileDownload()
                {
                    Url = _release.Url,
                    Filename = System.IO.Path.Combine(txtSavePath.Text,
                                                      System.IO.Path.GetFileName(Release.Url))
                });

                if (!downloader.Abort)
                {
                    // Vérifier le hash
                    Crc32 crc = new Crc32();

                    crc.Update(File.ReadAllBytes(_setup));
                    if (crc.Value.ToString("X").CompareTo(Release.CRC) != 0)
                        throw new Exception(strings.Updater_CorruptFile);
                    NextStep();
                }
                else
                    throw new Exception(strings.Updater_DownloadCancel);
                
            }
            catch (Exception error)
            {
                txtDwlError.Text = error.Message;
                txtDwlError.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private void OnDownloadProgress(object sender, CIV.Common.DownloadProgressEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
                {
                    if (pbDownload.Maximum != e.File.Size)
                        pbDownload.Maximum = e.File.Size;
                    if (String.IsNullOrEmpty(_setup))
                        _setup = e.File.Filename;
                    pbDownload.Value = e.Downloaded;
                    
                    lblDownload.Content = String.Format("{0} / {1}",
                                                        Common.UnitsConverter.SIUnitToString(Convert.ToDouble(e.Downloaded) / 1024, SIUnitTypes.ko),
                                                        Common.UnitsConverter.SIUnitToString(Convert.ToDouble(e.File.Size) / 1024, SIUnitTypes.ko));
                }));
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult choice = MessageBox.Show(strings.Updater_BackupMessage, strings.Updater_BackupTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            
            if (choice == MessageBoxResult.Yes)
            {
                try
                {
                    Backup backForm = new Backup();
                    backForm.OnEnd = EndBackup;
                    backForm.Show();
                }
                catch (Exception backupException)
                {
                    MessageBox.Show(backupException.Message);
                }
            }
            else if (choice == MessageBoxResult.No)
                LaunchInstall();
        }

        private void LaunchInstall()
        {
            if (!String.IsNullOrEmpty(_setup) && File.Exists(_setup))
            {
                System.Diagnostics.Process.Start(_setup);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void EndBackup(BackupEndEventArgs e)
        {
            if (!e.IsCancelled)
                LaunchInstall();
        }
    }
}
