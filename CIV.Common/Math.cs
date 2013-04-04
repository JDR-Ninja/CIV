using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class Math
    {
        public static double KoToMo(int value)
        {
            return value / 1024;
        }

        public static double KoToGo(int value)
        {
            return value / 1048576;
        }
    }
}
