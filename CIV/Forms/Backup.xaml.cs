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
using System.Threading;
using System.Windows.Threading;

namespace CIV
{
    /// <summary>
    /// Interaction logic for Backup.xaml
    /// </summary>
    public partial class Backup : Window
    {
        private delegate void LaunchBackupDelegate(string filename);
        private delegate void MethodInvokerNoArg();
        public delegate void BackupEndDelegate(BackupEndEventArgs e);

        public BackupEndDelegate OnEnd;

        private BackupManager _backup;

        public Backup()
        {
            InitializeComponent();
            this.Title = strings.Backup_Title;
        }

        private void OnProgress(object sender, BackupProgressEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (MethodInvokerNoArg)delegate()
            {
                lblTotal.Text = String.Format(strings.Backup_lblTotal,
                                                        e.Count,
                                                        e.TotalCount,
                                                        String.Format("{0:N2} ko", (double)e.TotalSize / 1024));
                pbTotal.Value = e.Progress;

                lblCurrentFile.Text = String.Format("{0}, {1}",
                                                        System.IO.Path.GetFileName(e.Filename),
                                                        String.Format("{0:N2} ko", (double)e.FileSize / 1024));
                pbCurrentFile.Value = e.FileProgress;
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dia = new System.Windows.Forms.SaveFileDialog();

            dia.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            dia.FileName = String.Format("civ-{0}.zip", DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));
            dia.Title = strings.Backup_SelectSaveFile;
            if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lblDestination.Text = dia.FileName;
                new LaunchBackupDelegate(LaunchBackup).BeginInvoke(dia.FileName, new AsyncCallback(CompletedWork), null);
            }
            else
                Close();
        }

        private void LaunchBackup(string filename)
        {
            try
            {
                _backup = new BackupManager();
                _backup.OnProgress += OnProgress;
                _backup.CreateBackup(filename);
            }
            catch (Exception backupException)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (MethodInvokerNoArg)delegate()
                {
                    MessageBox.Show(backupException.Message, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void CompletedWork(IAsyncResult result)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (MethodInvokerNoArg)delegate()
            {
                Close();
                if (OnEnd != null)
                    OnEnd(new BackupEndEventArgs(_backup.Cancel));
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_backup != null)
                _backup.Cancel = true;
        }
    }
}