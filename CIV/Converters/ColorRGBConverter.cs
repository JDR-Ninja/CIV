using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class ColorRGBConverter : ConverterMarkupExtension<ColorRGBConverter>
    {
        public ColorRGBConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ColorRGB colorValue = (ColorRGB)value;

            if (colorValue != null)
                return colorValue.GetSolidColorBrush();
            else
                return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
