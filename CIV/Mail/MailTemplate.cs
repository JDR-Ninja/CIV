using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CIV.Mail
{
    public class MailTemplate
    {
        public enum MailTemplateEncodingType { UTF8, ASCII, ISO8859 }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _templateFile;

        public string TemplateFile
        {
            get { return _templateFile; }
            set { _templateFile = value; }
        }

        private string _folder;

        [XmlIgnore]
        public string Folder
        {
            get { return _folder; }
            set { _folder = value; }
        }


        //private string _filename;

        [XmlIgnore]
        public string Filename
        {
            get { return System.IO.Path.Combine(Folder, TemplateFile); }
        }

        private bool _isHTML;

        public bool IsHTML
        {
            get { return _isHTML; }
            set { _isHTML = value; }
        }

        private MailTemplateEncodingType _encoding;

        public MailTemplateEncodingType Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        private string _Author;

        public string Author
        {
            get { return _Author; }
            set { _Author = value; }
        }

        public MailTemplate()
        {

        }

        public MailTemplate(string folder)
        {
            Folder = folder;
        }

        public string GetContent()
        {
            try
            {
                return File.ReadAllText(Filename, new UTF8Encoding());
            }
            catch (Exception loadException)
            {
                return "Erreur lors du chargement du modèle: " + loadException.Message;
            }
        }
    }
}
