using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CIV
{
    public class DoubleRangeValidationRule : BaseValidationRule
    {
        private double _min;

        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        private double _max;

        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }


        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            double doubleValue = (double)value;

            if (doubleValue >= Min && doubleValue <= Max)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, Message.Value);
            }
        }
    }
}
