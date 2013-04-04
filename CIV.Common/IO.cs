using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml;
using ICSharpCode.SharpZipLib.BZip2;

namespace CIV.Common
{
    public class IO
    {
        static public string GZipCompressString(string text, Encoding encoding)
        {
            return Convert.ToBase64String(GZipCompressArray(encoding.GetBytes(text)));
        }

        static public byte[] GZipCompressArray(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(data, 0, data.Length);
            }
            ms.Position = 0;
            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);
            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, gzBuffer, 0, 4);
            return gzBuffer;
        }

        static public string BZip2CompressString(string text, Encoding encoding)
        {
            return Convert.ToBase64String(BZip2CompressArray(encoding.GetBytes(text)));
        }

        static public byte[] BZip2CompressArray(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            using (BZip2OutputStream bz = new BZip2OutputStream(ms, 9))
            {
                bz.IsStreamOwner = false;
                bz.Write(data, 0, data.Length);
            }
            ms.Position = 0;
            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);
            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, gzBuffer, 0, 4);
            return gzBuffer;
        }

        static public string DecompressString(string text, Encoding encoding)
        {
            return encoding.GetString(DecompressArray(Convert.FromBase64String(text)));
        }

        static public byte[] DecompressArray(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(data, 0);
                ms.Write(data, 4, data.Length - 4);
                byte[] buffer = new byte[msgLength];
                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return buffer;
            }
        }

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

        public static string DecryptString(string text, string password)
        {
            return System.Text.Encoding.Unicode.GetString(DecryptArray(Convert.FromBase64String(text), password));
        }

        public static byte[] DecryptArray(byte[] cipherData, string password)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        private static byte[] Crypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();

            // Algorithm. Rijndael is available on all platforms.
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            //CryptoStream is for pumping our data.
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        public static string CryptString(string text, string password)
        {
            return Convert.ToBase64String(CryptArray(Encoding.Unicode.GetBytes(text), password));
        }

        public static byte[] CryptArray(byte[] clearData, string password)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            return Crypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        private static string _civDataFolder;

        public static string GetCivDataFolder()
        {
            // Gestion de CommonApplicationData et ApplicationData

            if (String.IsNullOrEmpty(_civDataFolder))
            {

                string folder;

                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portable")))
                    folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data\\");
                else
                    folder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "CIV\\v3") + Path.DirectorySeparatorChar;

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                _civDataFolder = folder;

                return folder;
            }
            else
                return _civDataFolder;
        }

    }
}
