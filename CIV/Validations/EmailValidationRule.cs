using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Controls;

namespace CIV
{
    public class EmailValidationRule : BaseValidationRule
    {
        private bool _isMultiple;

        public bool IsMultiple
        {
            get { return _isMultiple; }
            set { _isMultiple = value; }
        }

        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, Message.Value);

            string address = (string)value;

            if (String.IsNullOrEmpty(address))
                return new ValidationResult(false, Message.Value);

            string emailRegex = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";

            if (!IsMultiple || address.IndexOf(';') == -1)
            {
                if (!Regex.Match(address, emailRegex, RegexOptions.IgnoreCase).Success)
                    return new ValidationResult(false, Message.Value);
            }
            else
            {
                foreach (string token in address.Split(';'))
                    if (!Regex.Match(token, emailRegex, RegexOptions.IgnoreCase).Success)
                        return new ValidationResult(false, Message.Value);
            }

            return new ValidationResult(true, null);
        }
    }
}
