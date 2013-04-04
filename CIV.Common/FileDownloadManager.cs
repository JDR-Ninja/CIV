using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;

namespace CIV.Common
{
    public class FileDownloadManager
    {
        public delegate void DownloadProgressDelegate(object sender, DownloadProgressEventArgs e);

        public DownloadProgressDelegate OnDownloadProgress;

        private FileDownload _file;
        private int _bufferSize = 65536;//65536

        public int Timeout = 10000;
        public string UserAgent;

        public bool Abort = false;

        public void DoDownloadProgress(DownloadProgressEventArgs e)
        {
            if (OnDownloadProgress != null)
                OnDownloadProgress(this, e);
        }

        /// <summary>
        /// Incrémente un nom de fichier avec une valeur numérique
        /// </summary>
        /// <param name="filename">Le nom complèt du fichier (incluant le chemin)</param>
        /// <returns>Le nom du fichier, avec une incrémentation s'il y a lieu</returns>
        public string IncrementFilename(string filename)
        {
            // Si le fichier n'existe pas, on garde le même nom
            if (!File.Exists(filename))
                return filename;

            // Le chemin
            string path = System.IO.Path.GetDirectoryName(filename);
            // L'extension
            string ext = System.IO.Path.GetExtension(filename);
            // Le nom du fichier sans l'extension
            string name = System.IO.Path.GetFileName(filename).Substring(0, System.IO.Path.GetFileName(filename).Length - ext.Length);
            
            int count = 0;
            string newFilename;

            do
            {
                count++;
                newFilename = String.Format("{0}({1}){2}", System.IO.Path.Combine(path, name), count, ext);
            } while (File.Exists(newFilename));

            return newFilename;
        }

        public void Execute(FileDownload file)
        {
            Abort = false;
            _file = file;

            //HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(_release.Url);
            HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(_file.Url);
            newRequest.Timeout = Timeout;
            //newRequest.Host = "www.videotron.com";
            //newRequest.Referer = referer;
            newRequest.ProtocolVersion = HttpVersion.Version10;
            newRequest.UserAgent = UserAgent;
            newRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            newRequest.Proxy = HttpWebRequest.DefaultWebProxy;
            newRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            newRequest.KeepAlive = true;
            newRequest.AllowAutoRedirect = true;


            using (HttpWebResponse response = (HttpWebResponse)newRequest.GetResponse())
            {
                _file.Size = response.ContentLength;
                // Création d'un fichier vide pour réserver la place
                _file.Filename = IncrementFilename(_file.Filename);
                _file.Temp = IncrementFilename(_file.Filename + ".tmp");
                File.WriteAllText(_file.Filename, string.Empty);

                
                // Écriture dans un fichier temporaire
                using (FileStream fileStream = new FileStream(_file.Temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        using (Stream reader = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[_bufferSize];
                            int count;
                            long downloaded = 0;
                            
                            do
                            {
                                count = reader.Read(buffer, 0, buffer.Length);
                                writer.Write(buffer, 0, count);
                                downloaded += count;
                                DoDownloadProgress(new DownloadProgressEventArgs(_file, downloaded));
                            } while (count > 0 && !Abort);

                        }
                        writer.Flush();
                    }
                }
            }

            if (Abort)
            {
                File.Delete(_file.Filename);
                File.Delete(_file.Temp);
            }
            else
            {
                // Le fichier est téléchargé, il faut le renommer
                File.Delete(_file.Filename);
                File.Move(_file.Temp, _file.Filename);
            }

            
        }
    }
}
