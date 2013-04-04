using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    /// <summary>
    /// Classe qui est initialisé par CentralCenter
    /// </summary>
    public class SecurityToken
    {
        public enum SecurityType { Addition, Subtraction, Multiplication, Division, Modulo}

        public string Host;
        public SecurityType Security;
        public double NumberA;
        public double NumberB;
        public double NumberC;
        public long Salt;

        public SecurityToken()
        {
            Random secur = new Random();
            switch (secur.Next(5))
            {
                case 0: Security = SecurityType.Addition; break;
                case 1: Security = SecurityType.Subtraction; break;
                case 2: Security = SecurityType.Multiplication; break;
                case 3: Security = SecurityType.Division; break;
                case 4: Security = SecurityType.Modulo; break;
            }

            NumberA = (double)secur.Next(Int32.MaxValue);
            NumberB = (double)secur.Next(Int32.MaxValue);
            NumberC = 0;

            Salt = DateTime.Now.Ticks;
        }
    }
}
