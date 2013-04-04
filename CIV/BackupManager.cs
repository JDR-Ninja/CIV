using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.DAL;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using CIV.Common;

namespace CIV
{
    public class BackupManager
    {
        public delegate void BackupProgressEventHandler(object sender, BackupProgressEventArgs e);
        public event BackupProgressEventHandler OnProgress;

        private int _bufferSize = 65536; //65ko

        public bool Cancel;

        private void DoProgress(BackupProgressEventArgs e)
        {
            if (OnProgress != null)
                OnProgress(this, e);
        }

        /// <summary>
        /// Crée une sauvegarge des données de l'utilisateur
        /// </summary>
        public void CreateBackup(string destination)
        {
            Cancel = false;
            // Ferme la connexion à la BD le temps d'effectuer la sauvegarde
            DataBaseFactory.Instance.GetConnection().Close();

            string[] inFiles = Directory.GetFiles(CIV.Common.IO.GetCivDataFolder(), "*", SearchOption.AllDirectories);

            long totalSize = 0;

            foreach (string item in inFiles)
            {
                FileInfo fileInfo = new FileInfo(item);
                totalSize += fileInfo.Length;
            }

            using (FileStream fileStream = new FileStream(destination, FileMode.CreateNew))
            {
                using (ZipOutputStream zipper = new ZipOutputStream(fileStream))
                {
                    zipper.SetLevel(9);
                    zipper.UseZip64 = UseZip64.Off;
                    long totalCurrentRead = 0;

                    for (int i = 0; i < inFiles.Length; i++)
                    {
                        // Enlever le chemin complet
                        ZipEntry entry = new ZipEntry(inFiles[i].Replace(CIV.Common.IO.GetCivDataFolder(), String.Empty));
                        entry.DateTime = DateTime.Now;

                        zipper.PutNextEntry(entry);

                        using (FileStream reader = new FileStream(inFiles[i], FileMode.Open, FileAccess.Read))
                        {
                            byte[] data = new byte[_bufferSize];
                            int dataRead;
                            long fileRead = 0;
                            do
                            {
                                dataRead = reader.Read(data, 0, _bufferSize);
                                zipper.Write(data, 0, dataRead);

                                fileRead += dataRead;
                                totalCurrentRead += dataRead;

                                DoProgress(new BackupProgressEventArgs(100 * totalCurrentRead / totalSize,
                                                                       100 * fileRead / reader.Length,
                                                                       inFiles[i],
                                                                       reader.Length,
                                                                       totalSize,
                                                                       i + 1,
                                                                       inFiles.Length));

                            } while (dataRead > 0 && !Cancel);
                        }
                        if (Cancel)
                            break;
                    }
                    zipper.Finish();
                }
            }

            if (Cancel && File.Exists(destination))
                File.Delete(destination);
        }

        /// <summary>
        /// Restaure une sauvegarde des données de l'utilisateur
        /// </summary>
        public bool RestoreBackup()
        {
            Cancel = false;
            using (System.Windows.Forms.OpenFileDialog dia = new System.Windows.Forms.OpenFileDialog())
            {
                dia.Filter = "*.zip|*.zip";
                if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Effacer tout les fichiers actuel
                    DataBaseFactory.Instance.GetConnection().Close();

                    using (FileStream fileStream = new FileStream(dia.FileName, FileMode.Open))
                    {
                        using (ZipInputStream unzipper = new ZipInputStream(fileStream))
                        {
                            ZipEntry theEntry;

                            while ((theEntry = unzipper.GetNextEntry()) != null && !Cancel)
                            {
                                string directoryName = Path.GetDirectoryName(theEntry.Name);
                                string fileName = Path.GetFileName(theEntry.Name);

                                Directory.CreateDirectory(System.IO.Path.Combine(Common.IO.GetCivDataFolder(), directoryName));

                                if (fileName != String.Empty)
                                {
                                    string newFilename = System.IO.Path.Combine(Common.IO.GetCivDataFolder(), theEntry.Name);

                                    if (File.Exists(newFilename))
                                        File.Delete(newFilename);

                                    using (FileStream streamWriter = File.Create(newFilename))
                                    {

                                        int size = _bufferSize;
                                        byte[] data = new byte[_bufferSize];

                                        while (true && !Cancel)
                                        {
                                            size = unzipper.Read(data, 0, data.Length);
                                            if (size > 0)
                                                streamWriter.Write(data, 0, size);
                                            else
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
