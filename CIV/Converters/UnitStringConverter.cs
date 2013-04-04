using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;

namespace CIV
{
    public class UnitStringConverter : ConverterMarkupExtension<UnitStringConverter>
    {
        public UnitStringConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return CIV.Common.UnitsConverter.UnitToString((double)value, (UnitTypes)parameter);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
