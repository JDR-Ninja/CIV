using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CIV
{
    public class StringLenValidation : BaseValidationRule
    {
        private int _minLen = 0;

        public int MinLen
        {
            get { return _minLen; }
            set { _minLen = value; }
        }

        private int _maxLen = 0;

        public int MaxLen
        {
            get { return _maxLen; }
            set { _maxLen = value; }
        }

        public StringLenValidation()
        {

        }

        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            if (MinLen > 0 && value == null)
                return new ValidationResult(false, Message.Value);

            string text = (string)value;

            // Si le texte est trop court
            if (MinLen > 0 && text.Length < MinLen)
                return new ValidationResult(false, Message.Value);

            // Si le texte est trop long
            else if (MaxLen > 0  && text.Length > MaxLen)
                return new ValidationResult(false, Message.Value);

            else
                return new ValidationResult(true, null);
        }
    }
}
