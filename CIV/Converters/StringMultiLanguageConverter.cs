using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Videotron;
using CIV.Common;

namespace CIV
{
    public class StringMultiLanguageConverter : ConverterMarkupExtension<StringMultiLanguageConverter>
    {
        public StringMultiLanguageConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StringMultiLanguage text = (StringMultiLanguage)value;

            if (text != null)
                return text[ProgramSettings.Instance.UserLanguage];
            else
                return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
