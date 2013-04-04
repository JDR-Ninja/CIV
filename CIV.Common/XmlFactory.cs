using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;

namespace CIV.Common
{
    public class XmlFactory
    {

        public static byte[] SaveToArray(object o, System.Type type, XmlFactorySettings settings)
        {
            // http://connect.microsoft.com/VisualStudio/feedback/details/422414/xmlserializer-crashes-if-environment-currentdirectory-is-invalid
            
            XmlSerializer s;

            try
            {
                s = new XmlSerializer(type);
            }
            catch (ExternalException serialException)
            {
                Environment.CurrentDirectory = Environment.GetEnvironmentVariable("TEMP");
                Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TEMP"));
                s = new XmlSerializer(type);
            }

            XmlWriterSettings xmlsettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = settings.IsIndented,
                IndentChars = "\t",
                NewLineHandling = NewLineHandling.Entitize
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlWriter w = XmlWriter.Create(memoryStream, xmlsettings))
                {
                    s.Serialize(w, o);
                    w.Flush();
                }

                byte[] content = memoryStream.ToArray();

                // Encryption et Compression
                if (!String.IsNullOrEmpty(settings.Password))
                    return CIV.Common.IO.CryptArray(CIV.Common.IO.GZipCompressArray(content), settings.Password);

                // Compression
                else if (settings.IsCompressed)
                    return CIV.Common.IO.GZipCompressArray(content);

                // Plain
                else
                    return content;
            }
        }

        public static void SaveToFile(object o, System.Type type, string filename,  XmlFactorySettings settings)
        {
            File.WriteAllBytes(filename, SaveToArray(o, type, settings));   
        }

        public static string SaveToString(object o, System.Type type, XmlFactorySettings settings)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(SaveToArray(o, type, settings));
        }

        public static object LoadFromArray(byte[] content, System.Type type, XmlFactorySettings settings)
        {   
            object result;

            // Encryption et Compression
            if (!String.IsNullOrEmpty(settings.Password))
                content = CIV.Common.IO.DecompressArray(CIV.Common.IO.DecryptArray(content, settings.Password));

            // Compression
            else if (settings.IsCompressed)
                content = CIV.Common.IO.DecompressArray(content);

            using (MemoryStream memoryStream = new MemoryStream(content))
            {
                using (XmlReader r = XmlReader.Create(memoryStream))
                {
                    // http://connect.microsoft.com/VisualStudio/feedback/details/422414/xmlserializer-crashes-if-environment-currentdirectory-is-invalid
                    XmlSerializer s;

                    try
                    {
                        s = new XmlSerializer(type);
                    }
                    catch (ExternalException serialException)
                    {
                        Environment.CurrentDirectory = Environment.GetEnvironmentVariable("TEMP");
                        Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TEMP"));
                        s = new XmlSerializer(type);
                    }

                    result = s.Deserialize(r);
                }
            }   
                
            return result;
        }

        public static object LoadFromString(string content, System.Type type, XmlFactorySettings settings)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return LoadFromArray(encoding.GetBytes(content), type, settings);
        }

        public static object LoadFromFile(System.Type type, string filename, XmlFactorySettings settings)
        {

            if (File.Exists(filename))
                return LoadFromArray(File.ReadAllBytes(filename), type, settings);
            else
                return null;
        }
    }
}
