using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CIV
{
    public class IntegerValidation : BaseValidationRule
    {
        private bool _isPositive;

        public bool IsPositive
        {
            get { return _isPositive; }
            set { _isPositive = value; }
        }

        private bool _isNotZero;

        public bool IsNotZero
        {
            get { return _isNotZero; }
            set { _isNotZero = value; }
        }


        protected override ValidationResult DoValidate(object value, CultureInfo cultureInfo)
        {
            int int32Value;
            
            // Tentative de conversion de la valeur
            if (!Int32.TryParse((string)value, out int32Value))
                return new ValidationResult(false, Message.Value);

            // S'il faut que la valeur soit positif
            if (IsPositive && int32Value < 0)
                return new ValidationResult(false, Message.Value);

            // S'il faut que la valeur soit différente de zéro
            if (IsNotZero && int32Value == 0)
                return new ValidationResult(false, Message.Value);

            return new ValidationResult(true, null);
        }
    }
}
