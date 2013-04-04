using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class MultiLangueValue
    {
        private SupportedLanguages _language;

        public SupportedLanguages Language
        {
            get { return _language; }
            set { _language = value; }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

    }
}
