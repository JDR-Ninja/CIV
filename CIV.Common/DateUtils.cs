using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class DateUtils
    {
        /// <summary>
        /// Vérifie si deux DateTime représente la même journée
        /// </summary>
        /// <param name="dateA">Une date</param>
        /// <param name="dateB">Une date</param>
        /// <returns>Retourne Vrai si les deux dates sont dans la même journeé</returns>
        public static bool IsSameDay(DateTime dateA, DateTime dateB)
        {
            return dateA.Subtract(dateB).TotalDays == 0;
        }
    }
}
