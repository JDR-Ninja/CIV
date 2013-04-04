using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.BOL
{
    public class CivInfoBO
    {
        private string _name;

        /// <summary>
        /// L'Identifiant de l'information
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _value;

        /// <summary>
        /// Représente des données sérialisé
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

    }
}
