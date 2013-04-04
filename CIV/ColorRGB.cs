using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;

namespace CIV
{
    public class ColorRGB : INotifyPropertyChanged
    {
        private byte _red;

        public byte Red
        {
            get { return _red; }
            set { _red = value; Notify("Red"); }
        }

        private byte _green;

        public byte Green
        {
            get { return _green; }
            set { _green = value; Notify("Green"); }
        }

        private byte _blue;

        public byte Blue
        {
            get { return _blue; }
            set { _blue = value; Notify("Blue"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public ColorRGB()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        public ColorRGB(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public SolidColorBrush GetSolidColorBrush()
        {
            return new SolidColorBrush(new Color() {A = 255, R = Red, G = Green, B = Blue });
        }

        public System.Drawing.Color GetColor()
        {
            return System.Drawing.Color.FromArgb(Red, Green, Blue);
        }
    }
}
