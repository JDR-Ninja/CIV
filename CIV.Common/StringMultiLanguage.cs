using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class StringMultiLanguage
    {

        public List<MultiLangueValue> Labels = new List<MultiLangueValue>();

        public string this[SupportedLanguages sl]
        {
            get
            {
                MultiLangueValue text = Labels.SingleOrDefault(p => p.Language == sl);
                
                if (text != null)
                    return text.Text;
                else
                    return String.Empty;
            }

            set
            {
                Labels.RemoveAll(p => p.Language == sl);
                Labels.Add(new MultiLangueValue() {Language = sl, Text = value});
            }
        }

    }
}
