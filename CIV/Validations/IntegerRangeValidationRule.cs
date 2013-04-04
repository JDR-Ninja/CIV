using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CIV
{
    public class IntegerRangeValidationRule : BaseValidationRule
    {
        private int _min;

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        private int _max;

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }


        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            int intValue = (int)value;

            if (intValue >= Min && intValue <= Max)
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
