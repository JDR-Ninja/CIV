using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Controls;

namespace CIV
{
    public class RegexValidationRule : BaseValidationRule
    {
        private string _pattern;
        private Regex _regex;

        public string Pattern
        {
            get { return _pattern; }
            set
            {
                _pattern = value;
                _regex = new Regex(_pattern, RegexOptions.IgnoreCase);
            }
        }

        public RegexValidationRule() : base()
        {
            
        }

        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            if (value == null || !_regex.Match(value.ToString()).Success)
            {
                return new ValidationResult(false, Message.Value);
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }

}
