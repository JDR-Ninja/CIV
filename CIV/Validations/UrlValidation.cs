using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CIV
{
    public class UrlValidation : BaseValidationRule
    {
        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            string address = (string)value;

            if (Regex.Match(address, @"^[a-z0-9]+([a-z0-9]*\.{1})+[a-z0-9]+$", RegexOptions.IgnoreCase).Success)
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
