using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CIV
{
    public class BooleanDependencyObject : DependencyObject
    {
        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(BooleanDependencyObject), new UIPropertyMetadata(false));
    }
}
