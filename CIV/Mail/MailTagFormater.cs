using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Videotron;
using CIV.Common;
using System.Collections;

namespace CIV.Mail
{
    /// <summary>
    /// Parcours du texte pour convertir les tags rencontrés avec les valeurs du compte associé
    /// </summary>
    public class MailTagFormater
    {
        private const string MAIL_TAG = @"\[(?<tag>{0})(\((?<param>[^\)]*)?\))?\]";
        private const string METHOD_PARAM = @"(?<method>[a-z]{3,})=(?<param>[^,]+)";
        private VideotronAccount _account;
        private MailTagFactory _factory;
        private List<string> _tags;

        /// <summary>
        /// Initialise le constructeur
        /// </summary>
        /// <param name="account">Le compte vidéotron à utiliser pour la conversion</param>
        public MailTagFormater(VideotronAccount account)
        {
            _account = account;
            _factory = new MailTagFactory(_account);

            _tags = new List<string>();
            _tags.Add("NAME");
            _tags.Add("USERNAME");
            _tags.Add("PERIOD_START");
            _tags.Add("PERIOD_END");
            _tags.Add("DAY_REMAINING");
            _tags.Add("UPLOAD_PERCENT");
            _tags.Add("UPLOAD");
            _tags.Add("DOWNLOAD_PERCENT");
            _tags.Add("DOWNLOAD");
            _tags.Add("TOTAL_COMBINED_PERCENT");
            _tags.Add("TOTAL_COMBINED_REMAINING");
            _tags.Add("TOTAL_COMBINED_MAX");
            _tags.Add("TOTAL_COMBINED");
            _tags.Add("SUGGEST_DAILY_USAGE");
            _tags.Add("OVERCHARGE");
            _tags.Add("NOW");
            _tags.Add("LAST_UPDATE");
            _tags.Add("AVERAGE_COMBINED");
            _tags.Add("SUGGEST_COMBINED");
            _tags.Add("ESTIMATE_COMBINED");
            //_tags.Add("ESTIMATE_TOTAL_COMBINED");
            _tags.Add("SUGGEST_COMBINED_PERCENT");
            _tags.Add("THEORY_DAILY_COMBINED");
            _tags.Add("THEORY_COMBINED");
            _tags.Add("THEORY_COMBINED_DIFFERENCE");
        }

        /// <summary>
        /// Format une valeur en fonction des paramètres défini
        /// </summary>
        /// <param name="tag">Le tag trouvé dans le texte</param>
        /// <param name="param">Chaine de caractère représentant les fonctions à appliquer</param>
        /// <returns>Le nouvelle valeur</returns>
        private string Format(string tag, string param)
        {
            MailTagTypes type = _factory.GetType(tag);
            if (type != MailTagTypes.None)
            {
                try
                {
                    // Si c'est une date
                    if (_factory.IsDateTime(type))
                    {
                        return FormatDateTime(type, param);
                    }
                    // Si c'est un double
                    else if (_factory.IsDouble(type))
                    {
                        return FormatDouble(type, param);
                    }
                    // Si c'est un pourcentage
                    else if (_factory.IsPercent(type))
                    {
                        return FormatPercent(type, param);
                    }
                    // Si c'est un integer
                    else if (_factory.IsInt(type))
                    {
                        return _factory.GetIntValue(type).ToString();
                    }
                    // Si c'est de la monnaie
                    else if (_factory.IsCurrency(type))
                    {
                        return FormatCurrency(type, param);
                    }
                    // Si c'est du texte
                    else
                    {
                        return _factory.GetStringValue(type);
                    }
                }
                catch (Exception formatExcep)
                {
                    return String.Format("({0})", formatExcep.Message);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Converti un tag de format DateTime en une valeur string
        /// </summary>
        /// <param name="type">Le nom du tag</param>
        /// <param name="param">Les méthodes</param>
        /// <returns>La valeur à insérer dans le texte</returns>
        private string FormatDateTime(MailTagTypes type, string param)
        {
            Match paramMatch = Regex.Match(param, METHOD_PARAM, RegexOptions.IgnoreCase);
            while (paramMatch.Success)
            {
                if (paramMatch.Groups["method"].Value.ToUpper() == "FORMAT")
                {
                    return _factory.GetDateTimeValue(type).ToString(paramMatch.Groups["param"].Value);
                }
                paramMatch = paramMatch.NextMatch();
            }

            return _factory.GetDateTimeValue(type).ToShortDateString();
        }

        private string FormatDouble(MailTagTypes type, string param)
        {
            double value = _factory.GetDoubleValue(type);
            SIUnitTypes unit = ProgramSettings.Instance.ShowUnitType;

            Match paramMatch = Regex.Match(param, METHOD_PARAM, RegexOptions.IgnoreCase);
            while (paramMatch.Success)
            {
                if (paramMatch.Groups["method"].Value.ToUpper() == "ROUND")
                    value = System.Math.Round(value, Int32.Parse(paramMatch.Groups["param"].Value));
                else if (paramMatch.Groups["method"].Value.ToUpper() == "UNIT")
                {
                    switch (paramMatch.Groups["param"].Value.ToUpper())
                    {
                        case "G": unit = SIUnitTypes.Go; break;
                        case "M": unit = SIUnitTypes.Mo; break;
                        case "K": unit = SIUnitTypes.ko; break;
                    }
                }
                paramMatch = paramMatch.NextMatch();
            }

            return UnitsConverter.SIUnitToString(value, unit);
        }

        private string FormatPercent(MailTagTypes type, string param)
        {
            double value = _factory.GetPercentValue(type) * 100;
            int round = 2;

            Match paramMatch = Regex.Match(param, METHOD_PARAM, RegexOptions.IgnoreCase);
            while (paramMatch.Success)
            {
                if (paramMatch.Groups["method"].Value.ToUpper() == "ROUND")
                    round = Int32.Parse(paramMatch.Groups["param"].Value);
                paramMatch = paramMatch.NextMatch();
            }

            value = System.Math.Round(value, round);

            return value.ToString();
        }

        private string FormatCurrency(MailTagTypes type, string param)
        {
            double value = _factory.GetCurrencyValue(type);

            return value.ToString("C");
        }

        /// <summary>
        /// Parcours un texte et transforme les tags rencontrés
        /// </summary>
        /// <param name="text">Le texte à analyser</param>
        /// <returns>Le texte avec les tags convertis en valeur</returns>
        public string Convert(string text)
        {
            StringBuilder result = new StringBuilder(text);

            Match tagMatch;

            foreach (string key in _tags)
            {
                int delta = 0;

                // %DOWNLOAD_PERCENT[format:ff gg,round:2]%
                tagMatch = Regex.Match(result.ToString(), String.Format(MAIL_TAG, key));

                // Le tag est présent dans le modèle
                while (tagMatch.Success)
                {
                    result.Remove(tagMatch.Index + delta, tagMatch.Length);
                    string newValue = Format(tagMatch.Groups["tag"].Value, tagMatch.Groups["param"].Value);
                    result.Insert(tagMatch.Index + delta, newValue);

                    if (tagMatch.Length > newValue.Length)
                        delta -= tagMatch.Length - newValue.Length;
                    else if (tagMatch.Length < newValue.Length)
                        delta += newValue.Length - tagMatch.Length;

                    tagMatch = tagMatch.NextMatch();
                }
            }

            return result.ToString();
        }
    }
}
