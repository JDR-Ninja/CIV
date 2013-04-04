using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CIV
{
    public class NotNullValidationRule : BaseValidationRule
    {
        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            if (value == null || String.IsNullOrEmpty((string)value))
                return new ValidationResult(false, Message.Value);
            else
                return new ValidationResult(true, null);
        }
    }
}
