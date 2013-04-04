using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;

namespace CIV
{
    public abstract class BaseValidationRule : ValidationRule
    {
        private BooleanDependencyObject _active;

        public BooleanDependencyObject Active
        {
            get { return _active; }
            set { _active = value; }
        }

        private StringDependencyObject _message;

        public StringDependencyObject Message
        {
            get { return _message; }
            set { _message = value; }
        }

        protected BaseValidationRule()
        {
            ValidatesOnTargetUpdated = true;
        }

        protected abstract ValidationResult DoValidate(object value, System.Globalization.CultureInfo cultureInfo);
        
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (Active == null || (Active != null && Active.Value))
                return DoValidate(value, cultureInfo);
            else
                return new ValidationResult(true, null);
        }
    }
}
