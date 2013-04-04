using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CIV
{
    public class StringDependencyObject : DependencyObject
    {

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(StringDependencyObject), new UIPropertyMetadata(string.Empty));

    }
}
